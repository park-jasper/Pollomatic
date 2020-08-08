using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MoreLinq;
using Newtonsoft.Json;
using Pollomatic.Domain.Models;
using Pollomatic.Domain.Services;
using Pollomatic.Domain.ViewModels;

namespace Pollomatic.Domain
{
    public class Pollomat
    {
        private readonly IStorageService _storageService;
        private PollSpecificationViewModel _pollVm;

        public const string indexKey = "url-index";
        public Pollomat(IStorageService storageService, PollSpecificationViewModel pollVm)
        {
            _storageService = storageService;
            _pollVm = pollVm;

            _pollVm.SaveNewWebsite += async (url, content, path) => await SaveWebsiteForPollingAsync(url, content, path);
        }

        public async Task SaveWebsiteForPollingAsync(string url, string content, IEnumerable<ChildFeature> path)
        {
            await _storageService.StoreAsync(url, JsonConvert.SerializeObject(new PollDefinition()
            {
                Content = content,
                Features = path.ToList(),
            }));
            var index = await GetSavedUrlsAsync();
            if (!index.Contains(url))
            {
                index.Add(url);
            }
            await StoreSavedUrlsAsync(index);
        }

        public async Task SaveNewContentAsync(string url, string content)
        {
            var def = await GetDefinitionAsync(url);
            def.Content = content;
            await _storageService.StoreAsync(url, JsonConvert.SerializeObject(def));
        }

        public async Task<PollDefinition> GetDefinitionAsync(string url)
        {
            return JsonConvert.DeserializeObject<PollDefinition>(await _storageService.LoadAsync(url));
        }

        public async Task RemoveDefinition(string url)
        {
            await _storageService.DeleteAsync(url);
            var index = await GetSavedUrlsAsync();
            index.Remove(url);
            await StoreSavedUrlsAsync(index);
        }

        public async Task<IList<string>> GetSavedUrlsAsync()
        {
            var content = await _storageService.LoadAsync(indexKey);
            if (!string.IsNullOrEmpty(content))
            {
                return JsonConvert.DeserializeObject<List<string>>(content);
            }
            else
            {
                return new List<string>();
            }
        }

        public Task StoreSavedUrlsAsync(IList<string> savedUrls)
        {
            return _storageService.StoreAsync(indexKey, JsonConvert.SerializeObject(savedUrls));
        }

        public async Task<PollResult> CheckUrlAsync(string url)
        {
            var def = await GetDefinitionAsync(url);
            var doc = await HtmlFacade.Download(url);
            var current = HtmlFacade.GetRootNode(doc);
            foreach (var feature in def.Features)
            {
                try
                {
                    current = ApplyFeature(current, feature);
                }
                catch (Exception)
                {
                    return new PollResult()
                    {
                        Url = url,
                        IsDifferent = true,
                        DifferingContent = "",
                    };
                }
            }
            var isDifferent = current.InnerText != def.Content;
            return new PollResult()
            {
                Url = url,
                IsDifferent = isDifferent,
                DifferingContent = current.InnerText,
            };
        }

        private static HtmlNode ApplyFeature(HtmlNode current, ChildFeature feature)
        {
            Func<HtmlNode, bool> where = _ => true;
            foreach (var dist in feature.DistinguishingFeatures)
            {
                where = AddWhere(where, dist);
            }
            IEnumerable<HtmlNode> nodes = Enumerable.Empty<HtmlNode>();
            switch (feature.RelationToChild)
            {
                case ChildFeature.Relation.Self:
                case ChildFeature.Relation.Before:
                case ChildFeature.Relation.After:
                    nodes = current.ChildNodes;
                    break;
                case ChildFeature.Relation.Child:
                    nodes = current.ChildNodes.SelectMany(cn => cn.ChildNodes ?? Enumerable.Empty<HtmlNode>());
                    break;
            }
            var relevantChildren = nodes.Where(HtmlNavigation.IsRelevant).Where(where);
            HtmlNode targetNode;
            switch (feature.ChildPosition)
            {
                case ChildFeature.Position.Single:
                    targetNode = relevantChildren.Single();
                    break;
                case ChildFeature.Position.Number:
                    targetNode = relevantChildren.Skip(feature.Ordinal).First();
                    break;
                default:
                    throw new NotImplementedException();
            }
            switch (feature.RelationToChild)
            {
                case ChildFeature.Relation.Self:
                    return targetNode;
                case ChildFeature.Relation.Child:
                    return targetNode.ParentNode;
                case ChildFeature.Relation.Before:
                    return current.ChildNodes.SkipUntil(c => c == targetNode).First();
                case ChildFeature.Relation.After:
                    return current.ChildNodes.TakeUntil(c => c == targetNode).SkipLast(1).Last();
                default:
                    throw new NotImplementedException();
            }
        }

        private static Func<HtmlNode, bool> AddWhere(Func<HtmlNode, bool> where, ChildFeature.Feature feature)
        {
            Func<HtmlNode, bool> compareValue;
            switch (feature.Type)
            {
                case ChildFeature.FeatureType.Name:
                    compareValue = node => node.Name == feature.Value;
                    break;
                case ChildFeature.FeatureType.Id:
                    compareValue = node => node.Attributes?.FirstOrDefault(x => x.Name == "id")?.Value == feature.Value;
                    break;
                case ChildFeature.FeatureType.Class:
                    compareValue = node =>
                    {
                        var classString = node.Attributes?.FirstOrDefault(x => x.Name == "class")?.Value;
                        if (string.IsNullOrEmpty(classString))
                        {
                            return false;
                        }
                        var featureValues = feature.Value.Split(' ');
                        return featureValues.All(f => classString.Contains(f));
                    };
                    break;
                default:
                    throw new NotImplementedException();
            }
            return node => where(node) && compareValue(node);
        }
    }
}

using HtmlAgilityPack;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Pollomatic.Domain.ViewModels;
using System;
using MoreLinq;
using Pollomatic.Domain.Extensions;
using System.Dynamic;
using System.Windows.Input;
using Newtonsoft.Json;
using Pollomatic.Domain.Commands;

namespace Pollomatic.Domain.Models
{
    public class HtmlNavigation : BaseViewModel
    {
        public HtmlNode Node { get; }
        public HtmlNavigation Child { get; private set; }
        public HtmlNavigation Parent { get; private set; }
        public IEnumerable<ChildFeature> Features { get; set; }
        public ChildFeature SelectedFeature { get; set; }

        private readonly ICommand _selectFeature;

        private HtmlNavigation(HtmlNode node, IEnumerable<ChildFeature> features)
        {
            Node = node;
            Features = features;
            _selectFeature = new CommandForwarding(sender =>
            {
                if (sender is ChildFeature feature)
                {
                    SelectedFeature = feature;
                }
            });
            features.ForEach(f => f.Select = _selectFeature);
        }

        public static HtmlNavigation Create(HtmlNode leaf)
        {
            HtmlNavigation current = new HtmlNavigation(leaf, Enumerable.Empty<ChildFeature>());
            var currentNode = leaf;
            while (!IsRoot(currentNode))
            {
                var features = GetFeatures(currentNode, currentNode.ParentNode).ToList();
                var next = new HtmlNavigation(currentNode, features);
                next.Child = current;
                current.Parent = next;
                current = next;
                currentNode = currentNode.ParentNode;
            }
            return current;
        }

        public bool IsLeaf => Child == null || !Child.Features.Any();

        private static bool IsRoot(HtmlNode node)
        {
            return node.Name == "html" || node.ParentNode == null;
        }

        public void GetSelections(IList<ChildFeature> featureList)
        {
            featureList.Add(SelectedFeature);
            if (!IsLeaf)
            {
                Child.GetSelections(featureList);
            }
        }

        private static IEnumerable<ChildFeature> GetFeatures(HtmlNode child, HtmlNode parent)
        {
            var siblings = parent.ChildNodes.Where(IsRelevant).ToList();
            siblings
                .TakePartialWhile(c => c != child)
                .InvokePartial(x => x.ToList(), out var siblingsBefore)
                .Skip(1)
                .Invoke(x => x.ToList(), out var siblingsAfter);
            if (siblings.Count == 1)
            {
                return MoreEnumerable.From(() => new ChildFeature()
                {
                    ChildPosition = ChildFeature.Position.Single,
                });
            }
            return GetFeatures(child, siblingsBefore, siblingsAfter, true);
        }
        private static IEnumerable<ChildFeature> GetFeatures(HtmlNode child, ICollection<HtmlNode> siblingsBefore, ICollection<HtmlNode> siblingsAfter, bool lookAtSiblings)
        {

            ChildFeature Extract<TProperty>(Func<HtmlNode, TProperty> getProperty,
                Func<TProperty, TProperty, bool> compare, ChildFeature.FeatureType featureType, string featureValue) =>
                ExtractFeature(child, siblingsBefore, siblingsAfter, getProperty, compare, featureType, featureValue);
            yield return Extract(node => node.Name, (left, right) => left == right, ChildFeature.FeatureType.Name, child.Name);
            var idAttribute = child.Attributes.FirstOrDefault(att => att.Name == "id");
            if (idAttribute != null)
            {
                yield return new ChildFeature()
                {
                    ChildPosition = ChildFeature.Position.Single,
                    DistinguishingFeatures =
                    {
                        new ChildFeature.Feature(ChildFeature.FeatureType.Id, idAttribute.Value),
                    },
                };
            }
            var classAttribute = child.Attributes.FirstOrDefault(att => att.Name == "class");
            if (classAttribute != null)
            {
                var classes = classAttribute.Value.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).WhereNot(string.IsNullOrWhiteSpace);
                foreach (var subset in classes.Subsets())
                {
                    yield return Extract(
                        node => node.Attributes.Where(att =>
                            att.Name == classAttribute.Name && subset.All(c => att.Value.Contains(c))),
                        (left, right) => left.Any() && right.Any(),
                        ChildFeature.FeatureType.Class,
                        string.Join(" ", subset));
                }
                //yield return Extract(node => node.Attributes.Where(att =>
                //        att.Name == classAttribute.Name && att.Value == classAttribute.Value),
                //    (left, right) => left.Any() && right.Any(),
                //    ChildFeature.FeatureType.Class, classAttribute.Value);
            }
            yield return new ChildFeature()
            {
                ChildPosition = ChildFeature.Position.Number,
                Ordinal = siblingsBefore.Count,
            };
            if (lookAtSiblings)
            {
                if (siblingsBefore.Any())
                {
                    var featuresBefore = GetFeatures(
                        siblingsBefore.Last(), 
                        siblingsBefore.SkipLast(1).ToList(), 
                        Enumerable.Prepend(siblingsAfter, child).ToList(), 
                        false);
                    foreach (var featureBefore in featuresBefore)
                    {
                        featureBefore.RelationToChild = ChildFeature.Relation.Before;
                        yield return featureBefore;
                    }
                }
                if (siblingsAfter.Any())
                {
                    var featuresAfter = GetFeatures(
                        siblingsAfter.First(), 
                        Enumerable.Append(siblingsBefore, child).ToList(),
                        siblingsAfter.Skip(1).ToList(),
                        false);
                    foreach (var featureAfter in featuresAfter)
                    {
                        featureAfter.RelationToChild = ChildFeature.Relation.After;
                        yield return featureAfter;
                    }
                }
                if (child.ChildNodes.Any(IsRelevant))
                {
                    foreach (var grandchild in child.ChildNodes.Where(IsRelevant))
                    {
                        foreach (var nestedFeature in GetFeatures(
                            grandchild, 
                            siblingsBefore.SelectMany(sib => sib.ChildNodes).ToList(),
                            siblingsAfter.SelectMany(sib => sib.ChildNodes).ToList(),
                            false))
                        {
                            nestedFeature.RelationToChild = ChildFeature.Relation.Child;
                            yield return nestedFeature;
                        }
                    }
                }
            }
        }

        private static ChildFeature ExtractFeature<TProperty>(
            HtmlNode child, 
            IEnumerable<HtmlNode> siblingsBefore,
            IEnumerable<HtmlNode> siblingsAfter,
            Func<HtmlNode, TProperty> getProperty, 
            Func<TProperty, TProperty, bool> compare,
            ChildFeature.FeatureType featureType,
            string featureValue)
        {
            bool EqualToChild(HtmlNode sibling) => compare(getProperty(sibling), getProperty(child));
            var withSameFeatureBefore = siblingsBefore.Count(EqualToChild);
            var withSameFeatureAfter = siblingsAfter.Count(EqualToChild);
            var pos = withSameFeatureBefore + withSameFeatureAfter == 0
                ? ChildFeature.Position.Single
                : ChildFeature.Position.Number;
            return new ChildFeature()
            {
                ChildPosition = pos,
                Ordinal = withSameFeatureBefore,
                DistinguishingFeatures =
                {
                    new ChildFeature.Feature(featureType, featureValue),
                },
            };
        }

        private static bool IsTextNode(HtmlNode node)
        {
            var textNodes = new List<string>()
            {
                "a", "p", "td", "h",
            };
            textNodes.AddRange(Enumerable.Range(1, 9).Select(x => $"h{x}"));
            return textNodes.Contains(node.Name);
        }

        public static bool IsRelevant(HtmlNode node)
        {
            switch (node.NodeType)
            {
                case HtmlNodeType.Element:
                    return true;
                case HtmlNodeType.Text:
                    return !string.IsNullOrWhiteSpace(node.InnerText);
                default:
                    return false;
            }
        }
    }

    public class ChildFeature
    {
        public Position ChildPosition { get; set; }
        public Relation RelationToChild { get; set; }
        public int Ordinal { get; set; }
        [JsonIgnore]
        public ICommand Select { get; set; }

        public ChildFeature()
        {
            RelationToChild = Relation.Self;
        }

        public List<Feature> DistinguishingFeatures { get; set; } =
            new List<Feature>();

        public enum Position
        {
            Number,
            Single
        }

        public enum FeatureType
        {
            Name,
            Class,
            Id,
        }

        public enum Relation
        {
            Self,
            Before,
            After,
            Child,
        }

        public class Feature
        {
            public FeatureType Type { get; }
            public string Value { get; }
            public Feature(FeatureType type, string value)
            {
                Type = type;
                Value = value;
            }
        }
    }
}
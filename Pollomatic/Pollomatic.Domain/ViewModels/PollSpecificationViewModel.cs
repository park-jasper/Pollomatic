using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HtmlAgilityPack;
using Pollomatic.Domain.Annotations;
using System.Linq;
using System.Xml.Linq;
using Pollomatic.Domain.Models;
using System.Windows.Input;
using Pollomatic.Domain.Commands;

namespace Pollomatic.Domain.ViewModels
{
    public class PollSpecificationViewModel : BaseViewModel
    {
        public ITreeItemViewModel HtmlRoot { get; set; }
        public ITreeItemViewModel SelectedItem { get; set; }
        public HtmlNavigation CurrentDecision { get; set; }
        public string CurrentDecisionText { get; set; }
        public string CurrentUri { get; set; }

        public event Action<string, string, List<ChildFeature>> SaveNewWebsite;

        public PollSpecificationViewModel()
        {
            PropertyChanged += PollSpecificationViewModel_PropertyChanged;
        }

        private void MakeDecision()
        {
            if (!CurrentDecision.IsLeaf)
            {
                SetDecision(CurrentDecision.Child);
            }
            else
            {
                var topLevelDecision = CurrentDecision;
                while (topLevelDecision.Parent != null)
                {
                    topLevelDecision = topLevelDecision.Parent;
                }
                var decisionHistory = new List<ChildFeature>();
                topLevelDecision.GetSelections(decisionHistory);
                SaveNewWebsite?.Invoke(CurrentUri, SelectedItem.Content, decisionHistory);
            }
        }

        public void SetDecision(HtmlNavigation decision)
        {
            if (CurrentDecision != null)
            {
                CurrentDecision.PropertyChanged -= CurrentDecisionPropertyChanged;
            }
            CurrentDecision = decision;
            if (CurrentDecision != null)
            {
                CurrentDecision.PropertyChanged += CurrentDecisionPropertyChanged;
            }
        }

        private void CurrentDecisionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(HtmlNavigation.SelectedFeature):
                    MakeDecision();
                    break;
            }
        }

        private void PollSpecificationViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CurrentDecision):
                    CurrentDecisionText = GetContent(CurrentDecision.Node);
                    break;
            }
        }

        public void Set(string uri, HtmlDocument htmlDoc)
        {
            CurrentUri = uri;
            var html = HtmlFacade.GetRootNode(htmlDoc);
            HtmlRoot = ParseChildren(html);
        }

        private ITreeItemViewModel ParseChildren(HtmlNode html)
        {
            var res = new HtmlTreeItemViewModel(html)
            {
                DisplayContent = GetContent(html),
            };
            foreach (var child in html.ChildNodes.Where(HtmlNavigation.IsRelevant))
            {
                res.Children.Add(ParseChildren(child));
            }
            return res;
        }

        private static string GetContent(HtmlNode node)
        {
            string content = "";
            if (node.NodeType == HtmlNodeType.Text)
            {
                content = node.InnerText;
            }
            else
            {
                switch (node.Name)
                {
                    case "div":
                    case "span":
                        if (node.ChildNodes.Count <= 1)
                        {
                            content = node.InnerText;
                        }
                        break;
                    case "a":
                    case "p":
                    case "td":
                    case "h":
                    case "h1":
                    case "h2":
                    case "h3":
                    case "h4":
                    case "h5":
                    case "h6":
                    case "h8":
                    case "h7":
                    case "h9":
                        content = node.InnerText;
                        break;
                }
            }
            var attributes = string.Join(" ", node.Attributes.Select(att => $"{att.Name}=\"{att.Value}\""));
            return $"<{node.Name} {attributes}> {content} </{node.Name}>";
        }
    }
}
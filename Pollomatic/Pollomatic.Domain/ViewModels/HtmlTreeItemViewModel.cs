using System.Collections;
using System.Collections.ObjectModel;
using HtmlAgilityPack;

namespace Pollomatic.Domain.ViewModels
{
    public class HtmlTreeItemViewModel : TreeItemViewModel
    {
        public HtmlNode Node { get; }
        public HtmlTreeItemViewModel(HtmlNode node)
        {
            Node = node;
            Children = new ObservableCollection<ITreeItemViewModel>();
            Content = node.InnerText;
        }
    }
}
using System.Collections.Generic;
using System.Collections.Specialized;
using Pollomatic.Domain;
using Pollomatic.Extensions;
using Xamarin.Forms;

namespace Pollomatic.TreeView
{
    public class TreeView : ScrollView
    {
        private static readonly BindablePropertyExtension<ITreeItemViewModel> RootPropertyExtension
            = BindablePropertyExtension.Create<ITreeItemViewModel, TreeView>(nameof(Root), RootChanged);
        public static readonly BindableProperty RootProperty = RootPropertyExtension.Property;

        private static readonly BindablePropertyExtension<ITreeItemViewModel> SelectedItemPropertyExtension =
            BindablePropertyExtension.Create<ITreeItemViewModel, TreeView>(nameof(SelectedItem));
        public static readonly BindableProperty SelectedItemProperty = SelectedItemPropertyExtension.Property;

        public ITreeItemViewModel Root
        {
            get => this.GetValue(RootPropertyExtension);
            set => this.SetValue(RootPropertyExtension, value);
        }

        public ITreeItemViewModel SelectedItem
        {
            get => this.GetValue(SelectedItemPropertyExtension);
            set => this.SetValue(SelectedItemPropertyExtension, value);
        }

        private TreeViewNode _rootNode;

        public TreeView()
        {

        }

        private void SetRoot(ITreeItemViewModel vm) 
        {
            Content = _rootNode = new TreeViewNode(vm, 0, SelectionMade);
        }

        private void SelectionMade(ITreeItemViewModel obj)
        {
            SelectedItem = obj;
            _rootNode.BubbleDownUnselect();
        }

        public static void RootChanged(TreeView self, ITreeItemViewModel oldValue,
            ITreeItemViewModel newValue)
        {
            self.SetRoot(newValue);
        }
    }
}
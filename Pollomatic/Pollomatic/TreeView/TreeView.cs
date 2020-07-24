using System.Collections.Generic;
using System.Collections.Specialized;
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

        public TreeView()
        {

        }

        private void SetRoot(ITreeItemViewModel vm) 
        {
            Content = new TreeViewNode(vm);
        }

        public static void RootChanged(BindableObject bindable, ITreeItemViewModel oldValue,
            ITreeItemViewModel newValue)
        {
            if (bindable is TreeView treeView)
            {
                treeView.SetRoot(newValue);
            }
        }
    }
}
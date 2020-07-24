using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Pollomatic.TreeView
{
    public class TreeViewNode : StackLayout
    {
        public TreeViewElement Self { get; set; }
        public StackLayout ChildArea { get; set; }
        public bool Expanded { get; set; } = false;

        private readonly ITreeItemViewModel _viewModel;
        private ICommand ExpandCollapseCommand;

        public TreeViewNode(ITreeItemViewModel vm)
        {
            _viewModel = vm;
            ExpandCollapseCommand = new Command(ExpandCollapse);
            Self = new TreeViewElement(ExpandCollapseCommand)
            {
                Text = vm.Content
            };
            ChildArea = new StackLayout();
            SetItems();
            Children.Add(Self);
            Children.Add(ChildArea);
            vm.PropertyChanged += ViewModelPropertyChanged;
            PropertyChanged += NodePropertyChanged;
        }

        private void ExpandCollapse()
        {
            Expanded = !Expanded;
        }

        private async void NodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Expanded):
                    Self.Expanded = Expanded;
                    await Task.Delay((int) TreeViewElement.AnimationDelay);
                    RenderItems();
                    break;
            }
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ITreeItemViewModel.Content):
                    Self.Text = _viewModel.Content;
                    break;
                case nameof(ITreeItemViewModel.Children):
                    SetItems();
                    break;
            }
        }

        public void SetItems()
        {
            Subscribe();
            ClearItems();
            RenderItems();
        }

        public void ClearItems()
        {
            ChildArea.Children.Clear();
        }

        public void RenderItems()
        {
            if (Expanded && _viewModel.Children != null)
            {
                foreach (var ele in _viewModel.Children)
                {
                    AddItem(ele);
                }
            }
            else
            {
                ClearItems();
            }
        }

        private void AddItem(ITreeItemViewModel vm)
        {
            var newItem = new TreeViewNode(vm)
            {
                Margin = new Thickness(30, 0, 0, 0),
            };
            ChildArea.Children.Add(newItem);
        }

        private void Subscribe()
        {
            if (_viewModel.Children == null)
            {
                return;
            }
            _viewModel.Children.CollectionChanged += CollectionChanged;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!Expanded)
            {
                return;
            }
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var ele in e.NewItems.OfType<ITreeItemViewModel>())
                    {
                        AddItem(ele);
                    }
                    break;
                default:
                    throw new System.NotImplementedException();
                    break;
            }
        }
    }
}
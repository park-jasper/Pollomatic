using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Pollomatic.Domain;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace Pollomatic.TreeView
{
    public class TreeViewNode : StackLayout
    {
        public TreeViewElement Self { get; set; }
        public StackLayout ChildArea { get; set; }
        public bool Expanded { get; set; } = false;

        private readonly ITreeItemViewModel _viewModel;
        private readonly Action<ITreeItemViewModel> _selectViewModel;
        private readonly int _level;

        public TreeViewNode(ITreeItemViewModel vm, int level, Action<ITreeItemViewModel> selectViewModel)
        {
            _viewModel = vm;
            _level = level;
            _selectViewModel = selectViewModel;
            Self = new TreeViewElement(new Command(ExpandCollapse), new Command(Select))
            {
                Text = vm.DisplayContent
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

        private void Select()
        {
            _selectViewModel?.Invoke(_viewModel);
        }

        private void BubbleUpSelect(ITreeItemViewModel vm)
        {
            _selectViewModel?.Invoke(vm);
        }

        public void BubbleDownUnselect()
        {
            Self.Unselect();
            foreach (var node in ChildArea.Children.OfType<Grid>().Select(g => g.Children.OfType<TreeViewNode>().First()))
            {
                node.BubbleDownUnselect();
            }
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
                case nameof(ITreeItemViewModel.DisplayContent):
                    Self.Text = _viewModel.DisplayContent;
                    break;
                case nameof(ITreeItemViewModel.Children):
                    SetItems();
                    break;
            }
        }

        public void SetItems()
        {
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
            var newItem = new Grid()
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition() { Width = 32 },
                    new ColumnDefinition() { Width = GridLength.Star },
                },
            };
            const int progression = 25;
            var levelColor = Color.FromHsv(progression * _level, 100, 60);
            newItem.Children.Add(new BoxView()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = 1,
                BackgroundColor = levelColor,
                Margin = new Thickness(0, -3),
            }, 0, 0);
            newItem.Children.Add(new TreeViewNode(vm, _level + 1, BubbleUpSelect), 1, 0);
            ChildArea.Children.Add(newItem);
        }
    }
}
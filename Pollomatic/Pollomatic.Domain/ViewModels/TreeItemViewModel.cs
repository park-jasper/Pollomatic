using System.Collections.ObjectModel;

namespace Pollomatic.Domain.ViewModels
{
    public class TreeItemViewModel : BaseViewModel, ITreeItemViewModel
    {
        public string DisplayContent { get; set; }
        public string Content { get; set; }
        public ObservableCollection<ITreeItemViewModel> Children { get; set; }
    }
}
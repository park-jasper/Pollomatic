using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Pollomatic.Domain
{
    public interface ITreeItemViewModel : INotifyPropertyChanged
    {
        string DisplayContent { get; set; }
        string Content { get; set; }
        ObservableCollection<ITreeItemViewModel> Children { get; set; }
    }
}
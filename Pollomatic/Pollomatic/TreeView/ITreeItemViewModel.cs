using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Pollomatic.Domain.ViewModels;

namespace Xamarin.Forms
{
    public interface ITreeItemViewModel : INotifyPropertyChanged
    {
        string Content { get; set; }
        ObservableCollection<ITreeItemViewModel> Children { get; set; }
    }
}
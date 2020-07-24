using System.Collections.Generic;
using System.Collections.ObjectModel;
using Pollomatic.Domain.ViewModels;
using Xamarin.Forms;

namespace Pollomatic.ViewModels
{
    public class TreeItemViewModel : BaseViewModel, ITreeItemViewModel
    {
        public string Content { get; set; }
        public ObservableCollection<ITreeItemViewModel> Children { get; set; }
    }
}
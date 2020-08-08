using Pollomatic.Domain.ViewModels;
using System.Windows.Input;

namespace Pollomatic.ViewModels
{
    public class NavigationViewModel : BaseViewModel
    {
        public ICommand GoToSpecification { get; set; }
        public ICommand GoToOverview { get; set; }
        public ICommand ChooseFile { get; set; }
        public NavigationViewModel()
        {

        }
    }
}
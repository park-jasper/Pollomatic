using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pollomatic.Contracts;
using Pollomatic.Domain;
using Pollomatic.Domain.ViewModels;
using Pollomatic.ViewModels;
using Pollomatic.Views;
using Xamarin.Forms;

namespace Pollomatic
{
    public partial class MainPage : MasterDetailPage
    {
        public MainPage(MainViewModel mvm)
        {
            InitializeComponent();
            BindingContext = mvm;
            xaml_Navigation.BindingContext = new NavigationViewModel()
            {
                GoToSpecification = new Command(() => Detail = new SpecificationPage()
                    { BindingContext = mvm }),
                GoToOverview = new Command(() => Detail = new OverviewPage(new OverviewViewModel(mvm))),
                ChooseFile = new Command(ChooseFile),
            };
            
        }

        private static async void ChooseFile(object sender)
        {
            var picker = DependencyService.Get<IFilePicker>();
            var file = await picker.PickFile();
            if (!string.IsNullOrEmpty(file))
            {
                await ((App) App.Current).Load(file);
            }
        }
    }
}

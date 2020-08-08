using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pollomatic.Domain.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pollomatic.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SpecificationPage : ContentPage
    {
        public SpecificationPage()
        {
            InitializeComponent();
        }


        private void Entry_Completed(object sender, EventArgs e)
        {
            var mvm = BindingContext as MainViewModel;
            mvm.DownloadCommand.Execute(sender);
        }
    }
}
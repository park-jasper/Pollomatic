using System;
using Pollomatic.Domain;
using Pollomatic.Domain.ViewModels;
using Pollomatic.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Pollomatic
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var mp = new MainPage();
            MainPage = mp;
            var vm = new PollSpecificationViewModel();
            mp.PollViewModel = vm;
            var pollomat = new Pollomat(new StorageService(), vm);
            pollomat.AddUrl("https://www.itp.kit.edu/courses");
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

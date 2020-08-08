using System;
using System.Threading.Tasks;
using Pollomatic.Contracts;
using Pollomatic.Domain;
using Pollomatic.Domain.Services;
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
#pragma warning disable 4014
            Load();
#pragma warning restore 4014
        }

        public async Task Load(string filename = null)
        {
            var pollVm = new PollSpecificationViewModel();
            IStorageService storage;
            if (string.IsNullOrEmpty(filename))
            {
                storage = new StorageService();
            }
            else
            {
                var fileAccess = DependencyService.Get<IFileAccessFactory>().Get(filename);
                var fileStorage = new FileStorageService(fileAccess);
                await fileStorage.Initialize();
                storage = fileStorage;
            }
            var pollomat = new Pollomat(storage, pollVm);
            var mvm = new MainViewModel(pollVm, pollomat);
            var mp = new MainPage(mvm);
            MainPage = mp;
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

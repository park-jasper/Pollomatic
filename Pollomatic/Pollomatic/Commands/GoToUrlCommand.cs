using System;
using System.Windows.Input;
using Pollomatic.Contracts;
using Pollomatic.Domain.Models;
using Xamarin.Forms;

namespace Pollomatic.Commands
{
    public class GoToUrlCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            if (parameter is string url)
            {
                var withHttp = HtmlFacade.AddHttp(url);
                if (Uri.IsWellFormedUriString(withHttp, UriKind.Absolute))
                {
                    await DependencyService.Get<IUriStartingService>().LaunchUriAsync(new Uri(withHttp));
                }
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}
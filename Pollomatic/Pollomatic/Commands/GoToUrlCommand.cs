using System;
using System.Windows.Input;
using Pollomatic.Contracts;
using Pollomatic.Domain.Commands;
using Pollomatic.Domain.Models;
using Xamarin.Forms;

namespace Pollomatic.Commands
{
    public class GoToUrlCommand : CommandForwarding
    {
        public GoToUrlCommand() : base(GoToUrl)
        {

        }

        private static async void GoToUrl(object parameter)
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
    }
}
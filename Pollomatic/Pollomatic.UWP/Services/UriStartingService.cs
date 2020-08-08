using System;
using System.Threading.Tasks;
using Pollomatic.Contracts;

namespace Pollomatic.UWP.Services
{
    public class UriStartingService : IUriStartingService
    {
        public async Task LaunchUriAsync(Uri uri)
        {
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
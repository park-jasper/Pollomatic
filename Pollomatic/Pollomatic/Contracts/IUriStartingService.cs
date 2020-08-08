using System;
using System.Threading.Tasks;

namespace Pollomatic.Contracts
{
    public interface IUriStartingService
    {
        Task LaunchUriAsync(Uri uri);
    }
}
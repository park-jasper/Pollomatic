using System.Threading.Tasks;
using Pollomatic.Domain.Services;
using Xamarin.Essentials;

namespace Pollomatic.Services
{
    public class StorageService : IStorageService
    {
        public Task StoreAsync(string key, string value)
        {
            Preferences.Set(key, value);
            return Task.CompletedTask;
        }

        public Task<string> LoadAsync(string key)
        {
            var content = Preferences.Get(key, "");
            return Task.FromResult(content);
        }

        public Task DeleteAsync(string key)
        {
            Preferences.Remove(key);
            return Task.CompletedTask;
        }
    }
}
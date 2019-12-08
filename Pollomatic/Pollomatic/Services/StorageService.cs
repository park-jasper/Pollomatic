using Pollomatic.Domain.Services;
using Xamarin.Essentials;

namespace Pollomatic.Services
{
    public class StorageService : IStorageService
    {
        public void Store(string key, string value)
        {
            Preferences.Set(key, value);
        }

        public string Load(string key)
        {
            return Preferences.Get(key, "");
        }
    }
}
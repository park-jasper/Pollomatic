using System.Threading.Tasks;

namespace Pollomatic.Domain.Services
{
    public interface IStorageService
    {
        Task StoreAsync(string key, string value);
        Task<string> LoadAsync(string key);
        Task DeleteAsync(string key);
    }
}
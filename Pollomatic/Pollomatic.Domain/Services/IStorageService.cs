namespace Pollomatic.Domain.Services
{
    public interface IStorageService
    {
        void Store(string key, string value);
        string Load(string key);
    }
}
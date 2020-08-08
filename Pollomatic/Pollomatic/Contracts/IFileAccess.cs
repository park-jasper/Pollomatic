using System.Threading.Tasks;

namespace Pollomatic.Contracts
{
    public interface IFileAccess
    {
        Task<bool> ExistsFileAsync();
        Task<string> ReadAsync(); 
        Task WriteAsync(string content);
    }
}
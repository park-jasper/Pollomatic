using System.Threading.Tasks;

namespace Pollomatic.Contracts
{
    public interface IFilePicker
    {
        Task<string> PickFile();
    }
}
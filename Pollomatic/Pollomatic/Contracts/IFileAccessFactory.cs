using System.IO;

namespace Pollomatic.Contracts
{
    public interface IFileAccessFactory
    {
        IFileAccess Get(string fileName);
    }
}
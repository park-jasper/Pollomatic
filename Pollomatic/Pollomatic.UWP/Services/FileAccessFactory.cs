using Pollomatic.Contracts;

namespace Pollomatic.UWP.Services
{
    public class FileAccessFactory : IFileAccessFactory
    {
        public IFileAccess Get(string fileName)
        {
            return new FileAccess(fileName);
        }
    }
}
using System;
using System.Text;
using System.Threading.Tasks;
using Pollomatic.Contracts;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Pollomatic.UWP.Services
{
    public class FileAccess : IFileAccess
    {
        private string _fileName;
        public FileAccess(string fileName)
        {
            _fileName = fileName;
        }
        public async Task<bool> ExistsFileAsync()
        {
            var file = await StorageFile.GetFileFromPathAsync(_fileName);
            return file.IsAvailable;
        }

        public async Task<string> ReadAsync()
        {
            var file = await StorageFile.GetFileFromPathAsync(_fileName);
            var content = await FileIO.ReadTextAsync(file);
            return content;
        }

        public async Task WriteAsync(string content)
        {
            var file = await StorageFile.GetFileFromPathAsync(_fileName);
            await FileIO.WriteTextAsync(file, content);
        }
    }
}
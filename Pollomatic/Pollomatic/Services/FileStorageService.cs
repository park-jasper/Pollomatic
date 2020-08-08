using Newtonsoft.Json;
using Pollomatic.Contracts;
using Pollomatic.Domain.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pollomatic.Services
{
    public class FileStorageService : IStorageService
    {
        private readonly IFileAccess _fileAccess;
        private const string AppName = "Pollomatic";
        private object _fileLockHandle = new object();
        public FileStorageService(IFileAccess fileAccess)
        {
            _fileAccess = fileAccess;
        }
        public async Task StoreAsync(string key, string value)
        {
            var content = await GetContent();
            var entry = content.SingleOrDefault(e => e.Key == key);
            if (entry == null)
            {
                content.Add(new Entry
                {
                    Key = key,
                    Content = value,
                });
            }
            else
            {
                entry.Content = value;
            }
            await WriteContent(content);
        }

        public async Task<string> LoadAsync(string key)
        {
            var content = await GetContent();
            return content.FirstOrDefault(e => e.Key == key)?.Content;
        }

        public async Task DeleteAsync(string key)
        {
            var content = await GetContent();
            var entry = content.FirstOrDefault(e => e.Key == key);
            if (entry != null)
            {
                content.Remove(entry);
            }
            await WriteContent(content);
        }

        public async Task Initialize()
        {
            var emptyList = JsonConvert.SerializeObject(new List<Entry>(), Formatting.Indented);
            var exists = await _fileAccess.ExistsFileAsync();
            if (!exists)
            {
                await _fileAccess.WriteAsync(emptyList);
            }
            else
            {
                var content = await _fileAccess.ReadAsync();
                bool isValid = false;
                try
                {
                    var entries = JsonConvert.DeserializeObject<List<Entry>>(content);
                    isValid = entries != null;
                }
                catch { }
                if (!isValid)
                {
                    await _fileAccess.WriteAsync(emptyList);
                }
            }
        }

        private async Task<List<Entry>> GetContent()
        {
            string text = await _fileAccess.ReadAsync();
            return JsonConvert.DeserializeObject<List<Entry>>(text);
        }

        private async Task WriteContent(List<Entry> content)
        {
            var text = JsonConvert.SerializeObject(content, Formatting.Indented);
            await _fileAccess.WriteAsync(text);
        }

        private class Entry
        {
            public string Key { get; set; }
            public string Content { get; set; }
        }
    }
}
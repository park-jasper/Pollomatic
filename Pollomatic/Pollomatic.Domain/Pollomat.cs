using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Pollomatic.Domain.Services;
using Pollomatic.Domain.ViewModels;

namespace Pollomatic.Domain
{
    public class Pollomat
    {
        private readonly IStorageService _storageService;
        private PollSpecificationViewModel _pollVm;
        private static readonly HttpClient HttpClient = new HttpClient();
        public Pollomat(IStorageService storageService, PollSpecificationViewModel pollVm)
        {
            _storageService = storageService;
            _pollVm = pollVm;
        }

        public async Task AddUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                var uri = new Uri(url);
                var response = await HttpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var text = await response.Content.ReadAsStringAsync();
                    var doc = new HtmlDocument();
                    doc.LoadHtml(text);
                    _pollVm.Set(doc);
                }
            }
        }
    }
}

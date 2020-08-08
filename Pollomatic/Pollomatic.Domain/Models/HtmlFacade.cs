using System;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;
using System.Threading.Tasks;

namespace Pollomatic.Domain.Models
{
    public class HtmlFacade
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        public static async Task<HtmlDocument> Download(string url)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                return null;
            }
            var withHttp = AddHttp(url);
            var uri = new Uri(withHttp);
            var content = await HttpClient.GetAsync(uri);
            if (content.IsSuccessStatusCode)
            {
                var html = new HtmlDocument();
                var text = await content.Content.ReadAsStringAsync();
                html.LoadHtml(text);
                return html;
            }
            return null;
        }

        public static string AddHttp(string url)
        {
            if (!url.StartsWith("https://") && !url.StartsWith("http://"))
            {
                url = "http://" + url;
            }
            return url;
        }

        public static HtmlNode GetRootNode(HtmlDocument doc)
        {
            return doc.DocumentNode.ChildNodes.First(child => child.Name == "html");
        }
    }
}
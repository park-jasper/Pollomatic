using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HtmlAgilityPack;
using Pollomatic.Domain.Annotations;
using System.Linq;

namespace Pollomatic.Domain.ViewModels
{
    public class PollSpecificationViewModel : BaseViewModel
    {
        public List<TagViewModel> DisplayChildren { get; set; } = new List<TagViewModel>();

        public void Set(HtmlDocument htmlDoc)
        {
            var html = htmlDoc.DocumentNode.ChildNodes.First(child => child.Name == "html");
            DisplayChildren = ParseChildren(html);
        }

        private List<TagViewModel> ParseChildren(HtmlNode html)
        {
            var res = new List<TagViewModel>();
            foreach (var child in html.ChildNodes)
            {
                res.Add(new TagViewModel
                {
                    Name = child.Name,
                    Content = child.InnerText,
                    Children = ParseChildren(child)
                });
            }
            return res;
        }
    }
}
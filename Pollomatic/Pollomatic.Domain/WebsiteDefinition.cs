using System;
using System.Collections.Generic;

namespace Pollomatic.Domain
{
    public class WebsiteDefinition
    {
        public string Url { get; set; }
        public List<string> HtmlState { get; set; }
        public DateTime LastChanged { get; set; }
        public List<PollSpecification> Specifications { get; set; }
    }

    public class PollSpecification
    {
        public List<string> Path { get; set; }
    }
}
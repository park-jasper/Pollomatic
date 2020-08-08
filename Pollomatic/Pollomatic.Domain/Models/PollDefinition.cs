using System.Collections.Generic;
using System.Linq;

namespace Pollomatic.Domain.Models
{
    public class PollDefinition
    {
        public string Content { get; set; }
        public List<ChildFeature> Features { get; set; }

        public PollDefinition()
        {

        }
    }
}
namespace Pollomatic.Domain.Models
{
    public class PollResult
    {
        public string Url { get; set; }
        public bool IsDifferent { get; set; }
        public string DifferingContent { get; set; }
    }
}
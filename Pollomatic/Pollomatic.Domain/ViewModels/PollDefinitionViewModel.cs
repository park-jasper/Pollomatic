using System.Drawing;
using System.Windows.Input;
using Pollomatic.Domain.Models;

namespace Pollomatic.Domain.ViewModels
{
    public class PollDefinitionViewModel : BaseViewModel
    {
        public string Url { get; set; }
        public string Content { get; set; }
        public string OtherContent { get; set; }
        public Color StatusColor { get; set; }
        public bool IsScanning { get; set; } = false;
        public ICommand ReScanCommand { get; set; }
        public ICommand ApplyNewContent { get; set; }
        public ICommand RemoveCommand { get; set; }

        private readonly PollDefinition _definition;
        public PollDefinitionViewModel(string url, PollDefinition definition)
        {
            Url = url;
            Content = definition.Content;
            _definition = definition;
            StatusColor = Color.Transparent;
        }
    }
}
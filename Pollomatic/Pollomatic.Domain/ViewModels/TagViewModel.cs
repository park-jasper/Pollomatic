using System.Collections.Generic;
using System.Windows.Input;
using Pollomatic.Domain.Commands;

namespace Pollomatic.Domain.ViewModels
{
    public class TagViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public List<TagViewModel> Children = new List<TagViewModel>();
        public List<TagViewModel> DisplayChildren = new List<TagViewModel>();
        public bool Use { get; set; }

        private bool _isOpened = false;

        public ICommand OpenCloseGroup => new CommandForwarding(sender =>
        {
            if (_isOpened)
            {
                DisplayChildren.Clear();
            }
            else
            {
                DisplayChildren.AddRange(Children);
            }

            _isOpened = !_isOpened;
        });
    }
}
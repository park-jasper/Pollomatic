using System;
using System.Net.Http;
using System.Windows.Input;
using HtmlAgilityPack;
using Pollomatic.Domain.Commands;
using Pollomatic.Domain.Models;

namespace Pollomatic.Domain.ViewModels
{
    public class MainViewModel
    {
        public PollSpecificationViewModel PollViewModel { get; set; }
        public Pollomat Pollomat { get; set; }
        public ICommand DownloadCommand { get; set; }
        public ICommand ChooseCommand { get; set; }
        public string Url { get; set; }

        public MainViewModel(PollSpecificationViewModel pollVm, Pollomat pollomat)
        {
            PollViewModel = pollVm;
            Pollomat = pollomat;
            DownloadCommand = new CommandForwarding(Download);
            ChooseCommand = new CommandForwarding(Choose);
        }

        private async void Download(object sender)
        {
            var doc = await HtmlFacade.Download(Url);
            if (doc != null)
            {
                PollViewModel.Set(Url, doc);
            }
        }

        private void Choose(object sender)
        {
            var selected = PollViewModel.SelectedItem;
            if (selected is HtmlTreeItemViewModel htmlTreeVm)
            {
                var navigation = HtmlNavigation.Create(htmlTreeVm.Node);
                PollViewModel.SetDecision(navigation);
            }
        }
    }
}
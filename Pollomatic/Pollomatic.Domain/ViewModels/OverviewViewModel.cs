using MoreLinq;
using Pollomatic.Domain.Commands;
using Pollomatic.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pollomatic.Domain.ViewModels
{
    public class OverviewViewModel : BaseViewModel
    {
        private readonly Pollomat _pollomat;
        public ObservableCollection<PollDefinitionViewModel> Definitions { get; set; }
        public ICommand ScanAllCommand { get; set; }
        public OverviewViewModel(MainViewModel mvm)
        {
            _pollomat = mvm.Pollomat;
            ScanAllCommand = new CommandForwarding(ScanAll);
            Definitions = new ObservableCollection<PollDefinitionViewModel>();
#pragma warning disable 4014
            GetAvailableUrls();
#pragma warning restore 4014
        }

        private async void ScanAll(object obj)
        {
            foreach (var def in Definitions)
            {
                await ScanDefinition(def);
            }
        }

        private async Task ScanDefinition(PollDefinitionViewModel def)
        {
            def.IsScanning = true;
            var result = await _pollomat.CheckUrlAsync(def.Url);
            def.StatusColor = result.IsDifferent ? Color.Red : Color.Green;
            def.OtherContent = result.DifferingContent;
            def.IsScanning = false;
        }

        private async Task GetAvailableUrls()
        {
            var urls = await _pollomat.GetSavedUrlsAsync();
            foreach (var url in urls)
            {
                var def = await _pollomat.GetDefinitionAsync(url);
                var vm = new PollDefinitionViewModel(url, def);
                vm.ReScanCommand = new CommandForwarding(async sender => await ScanDefinition(vm));
                vm.ApplyNewContent = new CommandForwarding(
                    async sender =>
                    {
                        vm.Content = vm.OtherContent;
                        await _pollomat.SaveNewContentAsync(vm.Url, vm.Content);
                    },
                    sender => vm.OtherContent != null && vm.Content != vm.OtherContent);
                vm.PropertyChanged += (sender, e) =>
                {
                    switch (e.PropertyName)
                    {
                        case nameof(PollDefinitionViewModel.OtherContent):
                            ((CommandForwarding) vm.ApplyNewContent).OnCanExecuteChanged();
                            break;
                    }
                };
                vm.RemoveCommand = new CommandForwarding(async sender =>
                {
                    await _pollomat.RemoveDefinition(vm.Url);
                    Definitions.Remove(vm);
                });
                Definitions.Add(vm);
            }
        }
    }
}
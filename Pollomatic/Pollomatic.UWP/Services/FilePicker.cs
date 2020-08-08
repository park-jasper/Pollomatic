using System;
using System.Threading.Tasks;
using Windows.Storage.AccessCache;
using Pollomatic.Contracts;
using Windows.Storage.Pickers;

namespace Pollomatic.UWP.Services
{
    public class FilePicker : IFilePicker
    {
        public async Task<string> PickFile()
        {
            var picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            picker.FileTypeFilter.Add(".json");
            var result = await picker.PickSingleFileAsync();

            if (result != null && !StorageApplicationPermissions.FutureAccessList.CheckAccess(result))
            {
                StorageApplicationPermissions.FutureAccessList.Add(result);
            }

            return result?.Path;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pollomatic.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pollomatic.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PollSpecificationView : ContentView
	{
		public PollSpecificationView ()
		{
			InitializeComponent ();

            xaml_TreeView.Root = new TreeItemViewModel()
            {
                Content = "root",
                Children = new ObservableCollection<ITreeItemViewModel>()
                {
                    new TreeItemViewModel()
                    {
                        Content = "Hello",
                    },
                    new TreeItemViewModel()
                    {
                        Content = "Recurse",
                        Children = new ObservableCollection<ITreeItemViewModel>()
                        {
                            new TreeItemViewModel()
                            {
                                Content ="Child"
                            }
                        }
                    }
                }
            };
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                xaml_TreeView.Root.Children.Add(new TreeItemViewModel()
                {
                    Content = "Delayed" 
                });
            });
        }
	}
}
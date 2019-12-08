using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pollomatic.Domain;
using Pollomatic.Domain.ViewModels;
using Xamarin.Forms;

namespace Pollomatic
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public PollSpecificationViewModel PollViewModel { get; set; }
    }
}

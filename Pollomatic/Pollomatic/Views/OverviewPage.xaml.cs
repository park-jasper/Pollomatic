﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pollomatic.Domain.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pollomatic.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OverviewPage : ContentPage
    {
        public OverviewPage(OverviewViewModel ovm)
        {
            InitializeComponent();
            BindingContext = ovm;
        }
    }
}
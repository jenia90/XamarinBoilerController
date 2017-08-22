using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoilerController.ViewModels;
using BoilerController.Common.Converters;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BoilerController.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : ContentPage
	{
		public MainPage ()
		{
			InitializeComponent ();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindingContext = new MainPageViewModel();
        }
    }
}
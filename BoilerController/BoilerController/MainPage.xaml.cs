using System;
using BoilerController.ViewModels;
using Xamarin.Forms;

namespace BoilerController
{
	public partial class MainPage : ContentPage
	{
	    MainPageViewModel viewModel = new MainPageViewModel();

	    public MainPage()
		{
			InitializeComponent();
		    BindingContext = viewModel;
		}

	    private async void Button_OnClicked(object sender, EventArgs e)
	    {
	        var message = await viewModel.HttpHandlerTask("getled17");

            await DisplayAlert(message, message, "Cancel");
	    }
	}
}

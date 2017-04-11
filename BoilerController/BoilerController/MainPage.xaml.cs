using BoilerController.ViewModels;
using Xamarin.Forms;

namespace BoilerController
{
	public partial class MainPage : ContentPage
	{
	    

	    public MainPage()
		{
			InitializeComponent();
		    BindingContext = new MainPageViewModel();

		}
	}
}

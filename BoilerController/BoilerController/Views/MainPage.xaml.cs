using BoilerController.ViewModels;
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
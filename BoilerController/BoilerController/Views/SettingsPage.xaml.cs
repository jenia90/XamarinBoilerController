using BoilerController.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BoilerController.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage : ContentPage
	{
		public SettingsPage ()
		{
			InitializeComponent ();
            BindingContext = new SettingsPageViewModel();
		}
	}
}
using BoilerController.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BoilerController.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SchedulePage : ContentPage
	{
		public SchedulePage ()
		{
			InitializeComponent ();
            BindingContext = new SchedulePageViewModel();
		}
	}
}
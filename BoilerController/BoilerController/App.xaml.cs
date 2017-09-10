using BoilerController.Common.Helpers;
using BoilerController.Common.Services;
using BoilerController.Views;
using Xamarin.Forms;

namespace BoilerController
{
	public partial class App : Application
	{
        public static BoilerServerService Boiler;
		public App ()
		{
			InitializeComponent();
		    Boiler = new BoilerServerService(17);
            MainPage = new NavigationPage(new TabbedMainPage());
		}

		protected override void OnStart ()
		{
		    // Handle when your app starts
        }

        protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
            // Handle when your app resumes
		}
	}
}

using BoilerController.Common.Helpers;
using Xamarin.Forms;

namespace BoilerController
{
	public partial class App : Application
	{

	    public static NavigationPage CurrentPage;
		public App ()
		{
			InitializeComponent();

		    NetworkHandler.BaseUrl = Settings.ServerAddress;

            MainPage = new BoilerController.Views.MainPage();
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

using System;
using System.Windows.Input;
using BoilerController.ViewModels;
using Xamarin.Forms;

namespace BoilerController.Views
{
	public partial class MainPage : MasterDetailPage
    {
	    public MainPage()
		{
			InitializeComponent();
            Detail = new NavigationPage(new MainPageDetail());
		}

        private void ScheduleButton_OnClicked(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new SchedulePage());
            IsPresented = false;
        }

        private void SettingsButton_OnClicked(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new SettingsPage());
            IsPresented = false;
        }

        private void BackButton_OnClicked(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new MainPageDetail());
            IsPresented = false;
        }
    }
}

// Helpers/Settings.cs

using System;
using System.Text;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace BoilerController.Common.Helpers
{
	/// <summary>
	/// This is the Settings static class that can be used in your Core solution or in any
	/// of your client applications. All settings are laid out the same exact way with getters
	/// and setters. 
	/// </summary>
	public static class Settings
	{
	    private static ISettings AppSettings
	    {
	        get { return CrossSettings.Current; }
	    } 

		#region Setting Constants

		#endregion

	    public static string ServerAddress
	    {
	        get => AppSettings.GetValueOrDefault(nameof(ServerAddress), "192.168.1.178:5000");
	        set => AppSettings.AddOrUpdateValue(nameof(ServerAddress), value);
	    }

	    public static string Username
	    {
	        get => AppSettings.GetValueOrDefault(nameof(Username), string.Empty);
	        set => AppSettings.AddOrUpdateValue(nameof(Username), value);
        }

	    public static string Password
	    {
	        get => AppSettings.GetValueOrDefault(nameof(Password), string.Empty);
	        set => AppSettings.AddOrUpdateValue(nameof(Password), value);
        }

	}
}
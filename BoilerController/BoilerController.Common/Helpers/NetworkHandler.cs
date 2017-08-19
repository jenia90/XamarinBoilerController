using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BoilerController.Common.Helpers
{
    public class NetworkHandler
    {
        public static string BaseUrl { get; set; } = Settings.ServerAddress;

        /// <summary>
        ///     Sends formated request to the boiler server
        /// </summary>
        /// <param name="request" type="string">Request string to send</param>
        /// <returns>Return status from the server</returns>
        public static async Task<HttpResponseMessage> HttpRequestTask(string request, string json = "",
            string method = "GET")
        {
            HttpResponseMessage response;
            string requestUrl = "http://" + BaseUrl + "/api/";

            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(Settings.Username + ":" + Settings.Password)));

                    switch (method)
                    {
                        case "GET":
                            response = await client.GetAsync(new Uri(requestUrl + request));
                            break;
                        case "POST":
                            client.DefaultRequestHeaders.Accept.Add(
                                new MediaTypeWithQualityHeaderValue("application/json"));
                            response = await client.PostAsync(new Uri(requestUrl + request),
                                new StringContent(json, Encoding.UTF8, "application/json"));
                            break;
                        case "DELETE":
                            response = await client.DeleteAsync(new Uri(requestUrl + request));
                            break;
                        default:
                            response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                            break;
                    }
                }
                catch (HttpRequestException e)
                {
                    DisplayMessage("Server Unreachable", "Unable to connect to server");
                    return null;
                }
            }

            return response;
        }

        public static async void DisplayMessage(string title, string message, string cancel = "OK")
        {
            try
            {
                await Application.Current.MainPage.DisplayAlert(title, message, cancel);
            }
            catch (Exception e)
            {}
        }
    }
}
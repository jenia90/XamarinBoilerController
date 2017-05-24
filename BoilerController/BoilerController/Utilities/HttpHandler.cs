using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BoilerController.Utilities
{
    class HttpHandler
    {
        private static readonly string _baseurl = "http://192.168.1.120:5000/api/";
        //private static readonly string _baseurl = "http://192.168.1.178:5000/api/"; // uncomment for production

        /// <summary>
        /// Sends formated request to the boiler server
        /// </summary>
        /// <param name="request" type="HttpResponseMessage">Request string to send</param>
        /// <returns>Return status from the server</returns>
        public static  async Task<HttpResponseMessage> HttpRequestTask(string request, string json = "", string method = "GET")
        {
            HttpResponseMessage response;

            using (var client = new HttpClient())
            {
                try
                {
                    switch (method)
                    {
                        case "GET":
                            response = await client.GetAsync(_baseurl + request);
                            break;
                        case "POST":
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            response = await client.PostAsync(_baseurl + request, new StringContent(json, Encoding.UTF8, "application/json"));
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

        public static  async void DisplayMessage(string title, string message, string cancel = "OK")
        {
            await App.Current.MainPage.DisplayAlert(title, message, cancel);
        }
    }
}

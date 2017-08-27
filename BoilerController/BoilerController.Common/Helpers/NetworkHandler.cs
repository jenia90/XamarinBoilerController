using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BoilerController.Common.Helpers
{
    public class NetworkHandler
    {
        public static string BaseUrl { get; set; } = Settings.ServerAddress;

        /// <summary>
        /// Sends formatted request to the boiler server and returns the response
        /// </summary>
        /// <param name="request">Request string to be added to the base url</param>
        /// <param name="json">Json object string in case of POST request</param>
        /// <param name="method">HTTP method to use</param>
        /// <returns>HttpResponseMessage which was received from the server</returns>
        public static async Task<HttpResponseMessage> GetResponseTask(string request, string json = "",
                                                                      string method = "GET")
        {
            HttpResponseMessage response;
            string requestUrl = "http://" + BaseUrl + "/api/";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(Settings.Username + ":")) + Settings.Password);
                var uri = new Uri(requestUrl + request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                switch (method)
                {
                    case "GET":
                        response = await client.GetAsync(uri);
                        break;
                    case "POST":
                        client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));
                        response = await client.PostAsync(uri, content);
                        break;
                    case "PUT":
                        client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));
                        response = await client.PutAsync(uri, content);
                        break;
                    case "DELETE":
                        response = await client.DeleteAsync(uri);
                        break;
                    default:
                        response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                        break;
                }
            }

            return response;
        }
    }
}
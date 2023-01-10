using Medpharma.Mobile.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Medpharma.Mobile.Services
{
    public class ApiService : IApiService
    {
        public async Task<Response> Login<T>(string url, BindingLogin model)
        {
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                HttpClient client = new HttpClient(clientHandler);
                client.BaseAddress = new Uri(url);

                var json = JsonConvert.SerializeObject(model);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, data);

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }

                var list = JsonConvert.DeserializeObject<T>(result);
                return new Response
                {
                    IsSuccess = true,
                    Result = list
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response> GetListAsync<T>(string url)
        {
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                HttpClient client = new HttpClient(clientHandler);
                client.BaseAddress = new Uri(url);

                var model = new AuthIdentity();
                model.Email = Preferences.Get("user", "default_value");

                var json = JsonConvert.SerializeObject(model);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url),
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };

                var response = await client.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                //var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result
                    };
                }

                var list = JsonConvert.DeserializeObject<List<T>>(result);

                return new Response
                {
                    IsSuccess = true,
                    Result = list
                };

            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public static class VRWebAPIClient
    {
        public static Q Post<T, Q>(string url, string actionPath, T request, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                // New code:
                client.BaseAddress = new Uri(url);
                if (headers != null && headers.Count > 0)
                    AddHeaders(client, headers);
                var responseTask = client.PostAsJsonAsync(actionPath, request);
                responseTask.Wait();
                if (responseTask.Exception != null)
                    throw responseTask.Exception;
                if (responseTask.Result.IsSuccessStatusCode)
                {
                    var rsltTask = responseTask.Result.Content.ReadAsAsync<Q>();
                    rsltTask.Wait();
                    if (rsltTask.Exception != null)
                        throw rsltTask.Exception;
                    return rsltTask.Result;
                }
                else
                {
                    throw new Exception(String.Format("Error occured when calling action '{0}' on service '{1}'. Error: {2}", actionPath, url, responseTask.Result.ReasonPhrase));
                }
            }
        }

        public static Q Put<T, Q>(string url, string actionPath, T request, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                // New code:
                client.BaseAddress = new Uri(url);
                if (headers != null && headers.Count > 0)
                    AddHeaders(client, headers);
                var responseTask = client.PutAsJsonAsync(actionPath, request);
                responseTask.Wait();
                if (responseTask.Exception != null)
                    throw responseTask.Exception;
                if (responseTask.Result.IsSuccessStatusCode)
                {
                    var rsltTask = responseTask.Result.Content.ReadAsAsync<Q>();
                    rsltTask.Wait();
                    if (rsltTask.Exception != null)
                        throw rsltTask.Exception;
                    return rsltTask.Result;
                }
                else
                {
                    throw new Exception(String.Format("Error occured when calling action '{0}' on service '{1}'. Error: {2}", actionPath, url, responseTask.Result.ReasonPhrase));
                }
            }
        }

        public static T Get<T>(string url, string actionPath, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                // New code:
                client.BaseAddress = new Uri(url);
                if (headers != null && headers.Count > 0)
                    AddHeaders(client, headers);     
                var responseTask = client.GetAsync(actionPath);
                responseTask.Wait();
                if (responseTask.Exception != null)
                    throw responseTask.Exception;
                if (responseTask.Result.IsSuccessStatusCode)
                {
                    var rsltTask = responseTask.Result.Content.ReadAsAsync<T>();
                    rsltTask.Wait();
                    if (rsltTask.Exception != null)
                        throw rsltTask.Exception;
                    return rsltTask.Result;
                }
                else
                {
                    throw new Exception(String.Format("Error occured when calling action '{0}' on service '{1}'. Error: {2}", actionPath, url, responseTask.Result.ReasonPhrase));
                }
            }
        }

        private static void AddHeaders(HttpClient client, Dictionary<string, string> headers)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }
    }
}

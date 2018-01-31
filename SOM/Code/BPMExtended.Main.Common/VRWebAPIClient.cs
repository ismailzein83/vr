using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Common;

namespace BPMExtended.Main.Common
{
    public class MyPolicy : ICertificatePolicy
    {
        public bool CheckValidationResult(ServicePoint srvPoint, System.Security.Cryptography.X509Certificates.X509Certificate certificate, WebRequest request, int certificateProblem)
        {
            return true;
        }
    }

    public static class LocalWebAPIClient
    {
        static LocalWebAPIClient()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.CertificatePolicy = new MyPolicy();
        }
        public static Q Post<T, Q>(string url, string actionPath, T request, Dictionary<string, string> headers = null)
        {
            LocalWebAPIResponse<Q> response = Post<T, Q>(url, actionPath, request, false, headers);
            return response.Response;
        }

        public static LocalWebAPIResponse<Q> Post<T, Q>(string url, string actionPath, T request, bool withResponseHeaders, Dictionary<string, string> headers = null)
        {
            string serializedRequest = JSONSerializer.Serialize(request);
            using (var stringContent = new StringContent(serializedRequest, System.Text.Encoding.UTF8, "application/json"))
            {
                using (var client = new HttpClient())
                {
                    // New code:
                    client.BaseAddress = new Uri(url);
                    if (headers != null && headers.Count > 0)
                        AddHeaders(client, headers);

                    Task<HttpResponseMessage> responseTask = client.PostAsync(actionPath, stringContent);

                    responseTask.Wait();
                    if (responseTask.Exception != null)
                        throw responseTask.Exception;
                    if (responseTask.Result.IsSuccessStatusCode)
                    {
                        LocalWebAPIResponse<Q> response = new LocalWebAPIResponse<Q>();
                        if (withResponseHeaders)
                        {
                            response.Headers = new LocalWebAPIResponseHeader();
                            response.Headers.Location = responseTask.Result.Headers.Location;
                        }
                        var rsltTask = responseTask.Result.Content.ReadAsStringAsync();
                        rsltTask.Wait();
                        if (rsltTask.Exception != null)
                            throw rsltTask.Exception;
                        response.Response = JSONSerializer.Deserialize<Q>(rsltTask.Result);
                        return response;
                    }
                    else
                    {
                        throw new Exception(String.Format("Error occured when calling action '{0}' on service '{1}'. Error: {2}", actionPath, url, responseTask.Result.ReasonPhrase));
                    }
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
                    var rsltTask = responseTask.Result.Content.ReadAsStringAsync();
                    rsltTask.Wait();
                    if (rsltTask.Exception != null)
                        throw rsltTask.Exception;
                    return JSONSerializer.Deserialize<T>(rsltTask.Result);
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

    public class LocalWebAPIResponse<T>
    {
        public LocalWebAPIResponseHeader Headers { get; set; }
        public T Response { get; set; }
    }
    public class LocalWebAPIResponseHeader
    {
        public Uri Location { get; set; }
    }

}

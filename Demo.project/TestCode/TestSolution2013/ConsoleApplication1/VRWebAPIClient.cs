//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Formatting;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Threading.Tasks;

//namespace Vanrise.BPMOnline
//{

//    public class MyPolicy : ICertificatePolicy
//    {
//        public bool CheckValidationResult(ServicePoint srvPoint, System.Security.Cryptography.X509Certificates.X509Certificate certificate, WebRequest request, int certificateProblem)
//        {
//            return true;
//        }
//    }

//    public static class VRWebAPIClient
//    {
//        static VRWebAPIClient()
//        {
//            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
//            ServicePointManager.CertificatePolicy = new MyPolicy();
//        }
//        public static Q Post<T, Q>(string url, string actionPath, T request, Dictionary<string, string> headers = null, bool serializeWithFullType = false)
//        {
//            VRWebAPIResponse<Q> response = Post<T, Q>(url, actionPath, request, false, headers, serializeWithFullType);
//            return response.Response;
//        }

//        public static VRWebAPIResponse<Q> Post<T, Q>(string url, string actionPath, T request, bool withResponseHeaders, Dictionary<string, string> headers = null, bool serializeWithFullType = false)
//        {

//            using (var client = new HttpClient())
//            {
//                // New code:
//                client.BaseAddress = new Uri(url);
//                if (headers != null && headers.Count > 0)
//                    AddHeaders(client, headers);

//                Task<HttpResponseMessage> responseTask;
//                if (serializeWithFullType)
//                {
//                    var formatter = new JsonMediaTypeFormatter();
//                    formatter.SerializerSettings = new JsonSerializerSettings()
//                    {
//                        MissingMemberHandling = MissingMemberHandling.Ignore,
//                        NullValueHandling = NullValueHandling.Ignore,
//                        TypeNameHandling = TypeNameHandling.All
//                    };
//                    responseTask = client.PostAsync(actionPath, request, formatter);
//                }
//                else
//                    responseTask = client.PostAsJsonAsync(actionPath, request);

//                responseTask.Wait();
//                if (responseTask.Exception != null)
//                    throw responseTask.Exception;
//                if (responseTask.Result.IsSuccessStatusCode)
//                {
//                    VRWebAPIResponse<Q> response = new VRWebAPIResponse<Q>();
//                    if (withResponseHeaders)
//                    {
//                        response.Headers = new VRWebAPIResponseHeader();
//                        response.Headers.Location = responseTask.Result.Headers.Location;
//                    }
//                    var rsltTask = responseTask.Result.Content.ReadAsAsync<Q>();
//                    rsltTask.Wait();
//                    if (rsltTask.Exception != null)
//                        throw rsltTask.Exception;
//                    response.Response = rsltTask.Result;
//                    return response;
//                }
//                else
//                {
//                    throw new Exception(String.Format("Error occured when calling action '{0}' on service '{1}'. Error: {2}", actionPath, url, responseTask.Result.ReasonPhrase));
//                }
//            }
//        }

//        public static VRWebAPIResponse<string> Post(string url, string actionPath, string request, bool withResponseHeaders, Dictionary<string, string> headers = null)
//        {

//            using (var stringContent = new StringContent(request, System.Text.Encoding.UTF8, "application/json"))
//            {
//                using (var client = new HttpClient())
//                {
//                    // New code:
//                    client.BaseAddress = new Uri(url);
//                    if (headers != null && headers.Count > 0)
//                        AddHeaders(client, headers);

//                    Task<HttpResponseMessage> responseTask = client.PostAsync(actionPath, stringContent);

//                    responseTask.Wait();
//                    if (responseTask.Exception != null)
//                        throw responseTask.Exception;
//                    if (responseTask.Result.IsSuccessStatusCode)
//                    {
//                        VRWebAPIResponse<string> response = new VRWebAPIResponse<string>();
//                        if (withResponseHeaders)
//                        {
//                            response.Headers = new VRWebAPIResponseHeader();
//                            response.Headers.Location = responseTask.Result.Headers.Location;
//                        }
//                        var rsltTask = responseTask.Result.Content.ReadAsStringAsync();
//                        rsltTask.Wait();
//                        if (rsltTask.Exception != null)
//                            throw rsltTask.Exception;
//                        response.Response = rsltTask.Result;
//                        return response;
//                    }
//                    else
//                    {
//                        throw new Exception(String.Format("Error occured when calling action '{0}' on service '{1}'. Error: {2}", actionPath, url, responseTask.Result.ReasonPhrase));
//                    }
//                }
//            }
//        }
       

//        public static Q Put<T, Q>(string url, string actionPath, T request, Dictionary<string, string> headers = null)
//        {
//            using (var client = new HttpClient())
//            {
//                // New code:
//                client.BaseAddress = new Uri(url);
//                if (headers != null && headers.Count > 0)
//                    AddHeaders(client, headers);
//                var responseTask = client.PutAsJsonAsync(actionPath, request);
//                responseTask.Wait();
//                if (responseTask.Exception != null)
//                    throw responseTask.Exception;
//                if (responseTask.Result.IsSuccessStatusCode)
//                {
//                    var rsltTask = responseTask.Result.Content.ReadAsAsync<Q>();
//                    rsltTask.Wait();
//                    if (rsltTask.Exception != null)
//                        throw rsltTask.Exception;
//                    return rsltTask.Result;
//                }
//                else
//                {
//                    throw new Exception(String.Format("Error occured when calling action '{0}' on service '{1}'. Error: {2}", actionPath, url, responseTask.Result.ReasonPhrase));
//                }
//            }
//        }

//        public static T Get<T>(string url, string actionPath, Dictionary<string, string> headers = null)
//        {
//            using (var client = new HttpClient())
//            {
//                // New code:
//                client.BaseAddress = new Uri(url);
//                if (headers != null && headers.Count > 0)
//                    AddHeaders(client, headers);
//                var responseTask = client.GetAsync(actionPath);
//                responseTask.Wait();
//                if (responseTask.Exception != null)
//                    throw responseTask.Exception;
//                if (responseTask.Result.IsSuccessStatusCode)
//                {
//                    var rsltTask = responseTask.Result.Content.ReadAsAsync<T>();
//                    rsltTask.Wait();
//                    if (rsltTask.Exception != null)
//                        throw rsltTask.Exception;
//                    return rsltTask.Result;
//                }
//                else
//                {
//                    throw new Exception(String.Format("Error occured when calling action '{0}' on service '{1}'. Error: {2}", actionPath, url, responseTask.Result.ReasonPhrase));
//                }
//            }
//        }

//        private static void AddHeaders(HttpClient client, Dictionary<string, string> headers)
//        {
//            client.DefaultRequestHeaders.Accept.Clear();
//            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
//            foreach (var header in headers)
//            {
//                client.DefaultRequestHeaders.Add(header.Key, header.Value);
//            }
//        }
//    }

//    public static class VRSOMClient
//    {
//        public static string Post(string actionPath, string request)
//        {
//            string somBaseURL = "http://localhost:5559";
//            string authenticationRequest = "{\"Email\":\"admin@vanrise.com\",\"Password\":\"1\"}";
//            var result = VRWebAPIClient.Post(somBaseURL, "/api/VR_Sec/Security/Authenticate", authenticationRequest, false);
//            dynamic authenticationObj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result.Response).AuthenticationObject;
            
//            Dictionary<string, string> headers = new Dictionary<string, string>();
//            headers.Add(authenticationObj.TokenName.Value, authenticationObj.Token.Value);

//            return VRWebAPIClient.Post(somBaseURL, actionPath, request, false, headers).Response;

//        }
//    }

//    public enum CustomerObjectType { Account = 0, Contact = 1 }

//    public class SOMRequestManager
//    {
//        public void CreateRequest(CustomerObjectType customerObjectType, Guid accountOrContactId, string requestTitle, string requestContent)
//        {
//            //Insert Request into BPM
//            string createSOMRequest = String.Concat("{\"EntityId\":\"", customerObjectType.ToString(), "_", accountOrContactId.ToString(), "\",\"Settings\":{\"ExtendedSettings\":", requestContent, "}}");
//            VRSOMClient.Post("api/SOM_Main/SOMRequest/CreateSOMRequest", createSOMRequest);
//            //Update Process Instance Id in BPM
//        }


//    }

//    public class VRWebAPIResponse<T>
//    {
//        public VRWebAPIResponseHeader Headers { get; set; }
//        public T Response { get; set; }
//    }
//    public class VRWebAPIResponseHeader
//    {
//        public Uri Location { get; set; }
//    }
//}


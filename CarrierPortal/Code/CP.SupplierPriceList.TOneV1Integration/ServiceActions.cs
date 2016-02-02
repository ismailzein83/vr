using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace CP.SupplierPriceList.TOneV1Integration
{
    public class ServiceActions
    {
        public string Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public ServiceActions(string url, string password, string userName)
        {
            Url = url;
            UserName = userName;
            Password = password;
        }
        public RootObject GetAuthenticated()
        {
            RootObject tokenObject = null;
            string urls = string.Format("{0}/api/Security/authenticate?username={1}&password={2}", Url, UserName, Password);
            HttpWebRequest requests = (HttpWebRequest)WebRequest.Create(urls);
            requests.Method = "GET";
            try
            {
                var httpResponse = (HttpWebResponse)requests.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = streamReader.ReadToEnd();
                    tokenObject = JsonConvert.DeserializeObject<RootObject>(result);

                }
                httpResponse.Close();
            }
            catch (Exception e) { }
            return tokenObject;
        }

        public string ping(string token, string tokenName)
        {
            string res = "";
            string urls = string.Format("{0}/api/SupplierPriceList/ping", Url);
            HttpWebRequest requests = (HttpWebRequest)WebRequest.Create(urls);
            requests.Method = "GET";
            requests.Headers.Add(tokenName, token);
            try
            {
                var httpResponse = (HttpWebResponse)requests.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = streamReader.ReadToEnd();

                }
                httpResponse.Close();
            }
            catch (Exception e) { }
            return res;
        }
        public int UploadOnline(string token, string tokenName, SupplierPriceListConnector.SupplierPriceListUserInput userInput)
        {
            int insertedId;
            try
            {
                string URL = string.Format("{0}/api/SupplierPriceList/UploadPriceList", Url);
                string jSOnData = JsonConvert.SerializeObject(userInput);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = jSOnData.Length;
                request.Headers.Add(tokenName, token);
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(jSOnData);
                    streamWriter.Close();
                }
                try
                {
                    var httpResponse = (HttpWebResponse)request.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        string resultString = streamReader.ReadToEnd();
                        insertedId = int.Parse(resultString);
                        // object response = JsonConvert.DeserializeObject<object>(result);
                    }
                    httpResponse.Close();
                }

                catch (Exception e)
                {
                    insertedId = 0;
                    // throw e;
                }
            }
            catch (Exception)
            {
                insertedId = 0;
            }
            return insertedId;
        }

        public int GetResults(int queueId, string token, string tokenName)
        {
            int result;
            try
            {
                string URL = string.Format("{0}/api/SupplierPriceList/GetResults?QueueId={1}", Url, queueId);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = "GET";
                request.Headers.Add(tokenName, token);
                try
                {
                    var httpResponse = (HttpWebResponse)request.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        string resultString = streamReader.ReadToEnd();
                        result = int.Parse(resultString);
                    }
                    httpResponse.Close();
                }
                catch (Exception e)
                {
                    result = 0;
                    // throw e;
                }
            }
            catch (Exception)
            {
                result = 0;
                //throw;
            }
            return result;
        }
    }
}

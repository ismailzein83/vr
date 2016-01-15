using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace CP.SupplierPriceList.TOneV1Integration
{
    public class ServiceActions
    {
        public RootObject GetAuthenticated()
        {
            RootObject tokenObject = null;
            string urls = "http://localhost:7676/api/Security/authenticate?username=development@vanrise.com&password=123456";
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
            string urls = "http://localhost:7676/api/SupplierPriceList/ping";
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
        public bool UploadOnline(string token, string tokenName, SupplierPriceListConnector.SupplierPriceListUserInput userInput)
        {
            bool succ = true;
            try
            {
                string URL = "http://localhost:7676/api/SupplierPriceList/UploadPriceList";
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
                        string result = streamReader.ReadToEnd();
                        // object response = JsonConvert.DeserializeObject<object>(result);
                    }
                    httpResponse.Close();
                }

                catch (Exception e)
                {
                    succ = false;
                    // throw e;
                }
            }
            catch (Exception)
            {
                succ = false;
            }
            return succ;
        }
    }
}

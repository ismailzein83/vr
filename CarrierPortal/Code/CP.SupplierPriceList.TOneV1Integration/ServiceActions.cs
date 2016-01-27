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
        public int UploadOnline(string token, string tokenName, SupplierPriceListConnector.SupplierPriceListUserInput userInput)
        {
            int insertedId;
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
                string URL = "http://localhost:7676/api/SupplierPriceList/GetResults?QueueId=" + queueId;
                // string jSOnData = JsonConvert.SerializeObject(queueId);
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

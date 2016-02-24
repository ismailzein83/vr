using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using CP.SupplierPricelist.Entities;

namespace CP.SupplierPriceList.TOneV1Integration
{
    public class ServiceActions
    {
        private string Url { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }

        public ServiceActions(string url, string password, string userName)
        {
            Url = url;
            Username = userName;
            Password = password;
        }

        #region privateFunctions
        private string GetApiObject(string urls)
        {
            string result = String.Empty;
            HttpWebRequest requests = (HttpWebRequest)WebRequest.Create(urls);
            requests.Method = "GET";
            var httpResponse = (HttpWebResponse)requests.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            httpResponse.Close();
            return result;
        }
        private string GetApiObjectWithHeader(string urls)
        {
            RootObject tokenObject = GetAuthenticated();
            string result = String.Empty;
            if (tokenObject != null)
            {
                HttpWebRequest requests = (HttpWebRequest)WebRequest.Create(urls);
                requests.Method = "GET";
                requests.Headers.Add(tokenObject.TokenName, tokenObject.Token);
                var httpResponse = (HttpWebResponse)requests.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                httpResponse.Close();
            }
            return result;
        }

        private string PostObjectToApi(SupplierPriceListConnector.SupplierPriceListUserInput userInput)
        {
            RootObject tokenObject = GetAuthenticated();
            string resultString = String.Empty;
            if (tokenObject != null)
            {
                string url = string.Format("{0}/api/SupplierPriceList/UploadPriceList", Url);
                string jSOnData = JsonConvert.SerializeObject(userInput);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = jSOnData.Length;
                request.Headers.Add(tokenObject.TokenName, tokenObject.Token);
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(jSOnData);
                    streamWriter.Close();
                }
                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    resultString = streamReader.ReadToEnd();
                }
                httpResponse.Close();
            }
            return resultString;
        }
        #endregion

        public RootObject GetAuthenticated()
        {
            string urlsTemp = string.Format("{0}/api/Security/authenticate?username={1}&password={2}", Url, Username, Password);
            string result = GetApiObject(urlsTemp);
            return !string.IsNullOrEmpty(result) ? JsonConvert.DeserializeObject<RootObject>(result) : null;
        }

        public string Ping(string token, string tokenName)
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
        public int UploadOnline(SupplierPriceListConnector.SupplierPriceListUserInput userInput)
        {
            string result = PostObjectToApi(userInput);
            return !string.IsNullOrEmpty(result) ? int.Parse(result) : 0;
        }

        public QueueItemStatus GetResults(int queueId)
        {
            string urlTemp = string.Format("{0}/api/SupplierPriceList/GetResults?QueueId={1}", Url, queueId);
            string resultString = GetApiObject(urlTemp);
            QueueItemStatus queueItemStatus = (QueueItemStatus)int.Parse(resultString);
            return queueItemStatus;
        }
        public UploadInfo GetUploadInfo(int queueId)
        {
            string urlTemp = string.Format("{0}/api/SupplierPriceList/GetUploadInfo?QueueId={1}", Url, queueId);
            string resultString = GetApiObjectWithHeader(urlTemp);
            UploadInfo uploadInfo = JsonConvert.DeserializeObject<UploadInfo>(resultString);
            return uploadInfo;
        }

        public List<CarrierInfo> GetCarriersInfos()
        {
            string urlTemp = string.Format("{0}/api/BusinessEntity/GetCarriers?carrierType=2", Url);
            string resultString = GetApiObjectWithHeader(urlTemp);
            List<CarrierInfo> carrierInfos = JsonConvert.DeserializeObject<List<CarrierInfo>>(resultString);
            return carrierInfos;
        }
    }
}

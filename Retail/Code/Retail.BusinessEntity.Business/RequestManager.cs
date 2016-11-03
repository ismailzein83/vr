using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class RequestManager
    {
        public void PostRequest(string url, byte[] data,params string[]pars)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.MediaType = "application/json";
            request.ContentLength = data.Length;
            request.Accept = "application/json";
            request.Headers.Add("X-C5-Application", "f0106d37-e0d7-4ff4-9397-fd35e7608233");
            request.Headers.Add(HttpRequestHeader.Authorization, "Basic YWRtaW5AdnIucmVzdC53cy5kZTphZG1pbkB2cg==");
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Close();
            }
            var response = (HttpWebResponse)request.GetResponse();
            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            response.Close();

        }
        public void PutRequest(string url, byte[] data, params string[] pars)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.MediaType = "application/json";
            request.ContentLength = data.Length;
            request.Accept = "application/json";
            request.Headers.Add("X-C5-Application", "f0106d37-e0d7-4ff4-9397-fd35e7608233");
            request.Headers.Add(HttpRequestHeader.Authorization, "Basic YWRtaW5AdnIucmVzdC53cy5kZTphZG1pbkB2cg==");
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Close();
            }
            var response = (HttpWebResponse)request.GetResponse();
            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            response.Close();

        }
        public string GetRequest(string url, byte[] data)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "Get";
            request.ContentType = "application/json";
            request.MediaType = "application/json";
            request.ContentLength = data == null ? 0 : data.Length;
            request.Accept = "application/json";
            request.Headers.Add("X-C5-Application", "f0106d37-e0d7-4ff4-9397-fd35e7608233");
            request.Headers.Add(HttpRequestHeader.Authorization, "Basic YWRtaW5AdnIucmVzdC53cy5kZTphZG1pbkB2cg==");
            if (data != null)
            {
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                    stream.Close();
                }

            }

            var response = (HttpWebResponse)request.GetResponse();
            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            response.Close();
            return responseString;
        }
    }
}

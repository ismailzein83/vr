using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.iTestIntegration
{
    public class ServiceActions
    {
        public string PostRequest(string functionCode, string parameters)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.i-test.net/?t=" + functionCode + (parameters ?? ""));

            var postData = "email=myahya2@vanrise.com";
            postData += "&pass=123456789";
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            return new StreamReader(response.GetResponseStream()).ReadToEnd();
        }
    }
}

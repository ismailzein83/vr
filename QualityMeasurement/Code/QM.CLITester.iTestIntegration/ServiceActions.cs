using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace QM.CLITester.iTestIntegration
{
    public class ServiceActions
    {
        public string PostRequest(string functionCode, string parameters)
        {
            var request = (HttpWebRequest)WebRequest.Create(String.Format("{0}/?t={1}{2}", ConfigurationSettings.AppSettings["CliTester_ITest_Url"], functionCode, (parameters ?? "")));
            
            var postData = String.Format("email={0}&pass={1}", ConfigurationSettings.AppSettings["CliTester_ITest_Email"], ConfigurationSettings.AppSettings["CliTester_ITest_Password"]);
            
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return CleanResponse(responseString);
        }

        const string GoodAmpersand = "&amp;";

        public string CleanResponse(string response)
        {
            Regex badAmpersand = new Regex("&(?![a-zA-Z]{2,6};|#[0-9]{2,4};)");
            response = badAmpersand.Replace(response, GoodAmpersand);

            return response;
        }
    }
}

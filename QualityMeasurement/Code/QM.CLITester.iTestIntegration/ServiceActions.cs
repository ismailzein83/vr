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
            string decryptedConnectorUrl = Vanrise.Common.Cryptography.Decrypt(ConfigurationSettings.AppSettings["ConnectorUrl"], ConfigurationSettings.AppSettings["EncryptionSecretKey"]);
            string decryptedConnectorEmail = Vanrise.Common.Cryptography.Decrypt(ConfigurationSettings.AppSettings["ConnectorEmail"], ConfigurationSettings.AppSettings["EncryptionSecretKey"]);
            string decryptedConnectorPassword = Vanrise.Common.Cryptography.Decrypt(ConfigurationSettings.AppSettings["ConnectorPassword"], ConfigurationSettings.AppSettings["EncryptionSecretKey"]);

            var request = (HttpWebRequest)WebRequest.Create(String.Format("{0}/?t={1}{2}", decryptedConnectorUrl, functionCode, (parameters ?? "")));

            var postData = String.Format("email={0}&pass={1}", decryptedConnectorEmail, decryptedConnectorPassword);
            
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

        public string PostRequestBeta(string functionCode, string parameters)
        {
            string decryptedConnectorUrl = Vanrise.Common.Cryptography.Decrypt(ConfigurationSettings.AppSettings["ConnectorUrlBeta"], ConfigurationSettings.AppSettings["EncryptionSecretKey"]);
            string decryptedConnectorEmail = Vanrise.Common.Cryptography.Decrypt(ConfigurationSettings.AppSettings["ConnectorEmail"], ConfigurationSettings.AppSettings["EncryptionSecretKey"]);
            string decryptedConnectorPassword = Vanrise.Common.Cryptography.Decrypt(ConfigurationSettings.AppSettings["ConnectorPassword"], ConfigurationSettings.AppSettings["EncryptionSecretKey"]);

            var request = (HttpWebRequest)WebRequest.Create(String.Format("{0}/?t={1}{2}", decryptedConnectorUrl, functionCode, (parameters ?? "")));

            var postData = String.Format("email={0}&pass={1}", decryptedConnectorEmail, decryptedConnectorPassword);

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

        private string CleanResponse(string response)
        {
            Regex badAmpersand = new Regex("&(?![a-zA-Z]{2,6};|#[0-9]{2,4};)");
            response = badAmpersand.Replace(response, GoodAmpersand);

            return response;
        }
    }
}

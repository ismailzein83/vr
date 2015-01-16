using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
namespace TABS.Addons.Utilities
{
    public class WebServiceHelper
    {
        private string SoapMessage;
        private string UserName { get; set; }
        private string Password { get; set; }
        public string URL { get; set; }
        public string XMLResult { get; protected set; }
        private string Method { get; set; }

        public WebServiceHelper(string URL)
        {
            this.URL = URL;
        }
        public void Connect(string SoapMessage, ContentType ContentType)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);

            switch (ContentType)
            {
                case ContentType.SoapXML:
                    req.ContentType = "application/soap+xml;";
                    break;
                case ContentType.XML:
                    req.ContentType = "text/xml; charset=utf-8";
                    break;

            }
            req.Method = "POST";
            this.SoapMessage = SoapMessage;
            try
            {
                using (Stream stm = req.GetRequestStream())
                {
                    using (StreamWriter stmw = new StreamWriter(stm))
                    {
                        stmw.Write(this.SoapMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            WebResponse response = req.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {

                byte[] myData = ReadBinaryData(responseStream);
                this.XMLResult = System.Text.ASCIIEncoding.ASCII.GetString(myData);
            }
        }

        private byte[] ReadBinaryData(System.IO.Stream Stream)
        {
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = Stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                    {
                        ms.Dispose();
                        return ms.ToArray();
                    }
                    ms.Write(buffer, 0, read);
                }
            }
        }


        private Dictionary<Type, string> MappedTypes
        {
            get
            {
                Dictionary<Type, string> MappedTypes = new Dictionary<Type, string>();
                MappedTypes[typeof(int)] = "int";
                MappedTypes[typeof(string)] = "string";
                return MappedTypes;

            }

        }
        private string GetResult()
        {
            string methodResult = this.Method + "Result";
            string pattern = string.Format(@"(?<=<{0}>)(.)*(?=</{0}>)", methodResult);
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);
            var match = regex.Match(this.XMLResult);
            string result = match.Value;

            return result;
        }


    }
    public enum ContentType { SoapXML, XML }
}

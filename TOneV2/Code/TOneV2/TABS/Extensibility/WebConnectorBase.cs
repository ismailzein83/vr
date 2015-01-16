using System;
using System.Collections.Specialized;

namespace TABS.Extensibility
{
    public abstract class WebConnectorBase
    {
        protected string _URL;
        protected bool _useCompression = true;
        protected string _headerName;
        protected string _headerValue;
        protected NameValueCollection _Headers;

        public string this[string headerName] { get { return _Headers[headerName]; } set { _Headers[headerName] = value; } }

        protected WebConnectorBase()
        {
        }

        public WebConnectorBase(string URL, bool useCompression, string headerName, string headerValue)
        {
            _URL = URL;
            _Headers = new NameValueCollection();
            _useCompression = useCompression;
            _Headers[headerName] = headerValue;
            _Headers.Add(WebProxyBase.CompressionModeHeaderName, _useCompression ? WebProxyBase.GzipCompressionModeHeaderValue : "NONE");
        }

        /// <summary>
        /// Write to the proxy (defined by the URL) the contents of the provided stream
        /// </summary>
        /// <param name="stream">The stream from which to write</param>
        /// <returns>The response message as a string (Excluding the "Ok" / "Error" flags)</returns>
        public virtual string Write(System.IO.Stream stream, long estimatedLength, bool closeStream, log4net.ILog log)
        {
            string messageBody = string.Empty;
            try
            {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(_URL);

                request.Timeout = (int)TimeSpan.FromMinutes(60).TotalMilliseconds; // 1 hour is ok?

                byte[] buffer = new byte[1024 * 100];
                int bytesRead = 0;

                request.ContentLength = estimatedLength;

                if (_useCompression)
                {
                    log.InfoFormat("Compressing stream bytes (Estimated {0:0,000})", estimatedLength);
                    System.IO.MemoryStream compressed = new System.IO.MemoryStream();
                    System.IO.Compression.GZipStream gzip = new System.IO.Compression.GZipStream(compressed, System.IO.Compression.CompressionMode.Compress, true);

                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        gzip.Write(buffer, 0, bytesRead);
                        gzip.Flush();
                    }
                    if (closeStream) stream.Close();
                    closeStream = true;
                    gzip.Close();
                    stream = compressed;
                    stream.Position = 0;
                    request.ContentLength = compressed.Length;
                }
                
                log.InfoFormat("Request Length: {0:0,000} bytes", request.ContentLength);
                request.ContentType = "application/binary";
                foreach (string headerName in _Headers.AllKeys)
                    request.Headers.Add(headerName, _Headers[headerName]);
                request.KeepAlive = false;
                request.ProtocolVersion = System.Net.HttpVersion.Version11;
                request.Method = "POST";
                request.UserAgent = "WEB-CONNECTOR";

                log.Info("Sending Request...");

                System.IO.Stream requestStream = request.GetRequestStream();
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                    requestStream.Flush();
                }
                if (closeStream) stream.Close();
                requestStream.Close();

                log.Info("Recieving Response...");

                System.IO.StreamReader reader = new System.IO.StreamReader(request.GetResponse().GetResponseStream());

                string responseText = reader.ReadToEnd();

                log.InfoFormat("Recieved Response {0:0,000} characters", responseText.Length);

                bool isOK = responseText.StartsWith(WebProxyBase.ResponseOkMessage);                

                if (isOK)
                {
                    messageBody = responseText.Substring(WebProxyBase.ResponseOkMessage.Length);
                    log.InfoFormat("Write finished Successfully");
                }
                else
                {
                    messageBody = responseText.Substring(WebProxyBase.ResponseErrorMessage.Length);
                    log.Error("Write finished with Error", new Exception(messageBody));
                }
            }
            catch (Exception ex)
            {
                log.Error("Connector Write Error", ex);
            }
            return messageBody;
        }
    }
}
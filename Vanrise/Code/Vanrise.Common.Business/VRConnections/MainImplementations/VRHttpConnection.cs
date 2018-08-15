using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRHttpConnection : VRConnectionSettings
    {
        public override Guid ConfigId
        {
            get { return s_ConfigId; }
        }

        public static Guid s_ConfigId = new Guid("071D54D2-463B-4404-8219-45FCD539FF01");

        public string BaseURL { get; set; }

        public List<VRHttpHeader> Headers { get; set; }

        public List<VRWorkflowRetrySettings> WorkflowRetrySettings { get; set; }

        public virtual bool TrySendRequest(string actionPath, VRHttpMethod httpMethod, VRHttpMessageFormat messageFormat, Dictionary<string, string> urlParameters,
            Dictionary<string, string> headers, string body, Action<VRHttpResponse> onResponseReceived, bool throwIfError, Action<VRHttpFault> onError)
        {
            bool isSucceeded = false;
            VRHttpResponse response = null;
            VRHttpFault fault = null;
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.BaseAddress = new Uri(this.BaseURL);
                    string messageFormatValue = Utilities.GetEnumAttribute<VRHttpMessageFormat, VRHttpMessageFormatAttribute>(messageFormat).Value;
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(messageFormatValue));
                    string actionWithParameters = BuildActionWithParameters(actionPath, urlParameters);
                    using (System.Net.Http.HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(MapVRHttpMethod(httpMethod), actionWithParameters))
                    {
                        AddHeaders(request, headers);

                        if (!String.IsNullOrEmpty(body))
                            request.Content = new System.Net.Http.StringContent(body, System.Text.Encoding.UTF8, messageFormatValue);

                        using (var responseTask = client.SendAsync(request))
                        {
                            responseTask.Wait();
                            System.Net.Http.HttpResponseMessage responseMessage = responseTask.Result;
                            string responseString = responseMessage.Content.ReadAsStringAsync().Result;
                            if (responseTask.Exception == null && responseMessage.IsSuccessStatusCode)
                            {
                                isSucceeded = true;
                                response = new VRHttpResponse(responseString, responseMessage.StatusCode, messageFormat);
                            }
                            else
                            {
                                fault = new VRHttpFault(responseTask.Exception, responseString, responseMessage.StatusCode, messageFormat);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                onError(new VRHttpFault(ex));
                if (throwIfError)
                    throw;
            }
            if (fault != null)
            {
                onError(fault);
                if (throwIfError)
                    throw fault.Exception;
            }
            else if (response != null)
            {
                onResponseReceived(response);
            }
            return isSucceeded;
        }

        private System.Net.Http.HttpMethod MapVRHttpMethod(VRHttpMethod vrHttpMethod)
        {
            switch (vrHttpMethod)
            {
                case VRHttpMethod.Get: return System.Net.Http.HttpMethod.Get;
                case VRHttpMethod.Post: return System.Net.Http.HttpMethod.Post;
                case VRHttpMethod.Put: return System.Net.Http.HttpMethod.Put;
                case VRHttpMethod.Delete: return System.Net.Http.HttpMethod.Delete;
                default: throw new NotSupportedException(string.Format("unsupported vrHttpMethod '{0}'", vrHttpMethod.ToString()));
            }
        }

        private string BuildActionWithParameters(string actionPath, Dictionary<string, string> urlParameters)
        {
            actionPath.ThrowIfNull("actionPath");
            if (urlParameters != null && urlParameters.Count > 0)
                return string.Concat(actionPath, "?", string.Join("&", urlParameters.Select(itm => string.Concat(itm.Key, "=", itm.Value))));
            else
                return actionPath;
        }

        private void AddHeaders(System.Net.Http.HttpRequestMessage request, Dictionary<string, string> requestHeaders)
        {
            if (this.Headers != null)
            {
                foreach (var header in this.Headers)
                {
                    request.Headers.Add(header.Name, header.Value);
                }
            }
            if (requestHeaders != null)
            {
                foreach (var headerEntry in requestHeaders)
                {
                    request.Headers.Add(headerEntry.Key, headerEntry.Value);
                }
            }
        }
    }

    public class VRHttpConnectionFilter : IVRConnectionFilter
    {
        public bool IsMatched(VRConnection vrConnection)
        {
            vrConnection.ThrowIfNull("vrConnection");
            vrConnection.Settings.ThrowIfNull("vrConnection.Settings", vrConnection.VRConnectionId);

            if (vrConnection.Settings.ConfigId != VRHttpConnection.s_ConfigId)
                return false;

            return true;
        }
    }

    public class VRHttpHeader
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }

    public class VRWorkflowRetrySettings
    {
        public int MaxRetryCount { get;set;}

        public TimeSpan RetryInterval { get; set; }
    }

    public class VRHttpResponse
    {
        VRHttpMessageFormat _messageFormat;

        internal VRHttpResponse(string response, System.Net.HttpStatusCode statusCode, VRHttpMessageFormat messageFormat)
        {
            this.Response = response;
            this.StatusCode = statusCode;
            _messageFormat = messageFormat;
        }

        public string Response { get; private set; }

        public System.Net.HttpStatusCode StatusCode { get; private set; }

        public T DeserializeResponse<T>()
        {
            if (String.IsNullOrEmpty(this.Response))
            {
                return default(T);
            }
            else
            {
                switch (_messageFormat)
                {
                    case VRHttpMessageFormat.ApplicationJSON:
                        return Common.Serializer.Deserialize<T>(this.Response);
                    case VRHttpMessageFormat.ApplicationXML:
                    case VRHttpMessageFormat.TextXML:
                        return new VRXmlSerializer().Deserialize<T>(this.Response);
                    default: throw new NotSupportedException(String.Format("_messageFormat: '{0}'", _messageFormat.ToString()));
                }

            }
        }
    }

    public class VRHttpFault
    {
        VRHttpMessageFormat _messageFormat;

        string _errorMessage;
        Exception _exception;

        public VRHttpFault(Exception exception, string response, System.Net.HttpStatusCode statusCode, VRHttpMessageFormat messageFormat)
        {
            this.StringResponse = response;
            this.StatusCode = statusCode;
            _messageFormat = messageFormat;
            _exception = exception;
            if (exception != null)
                _errorMessage = _exception.Message;
        }

        public VRHttpFault(Exception exception)
        {
            _exception = exception;
            _errorMessage = exception.Message;
            this.StatusCode = System.Net.HttpStatusCode.ServiceUnavailable;
            this.StringResponse = "Request not sent!";
        }

        public string StringResponse { get; private set; }

        public string ErrorMessage
        {
            get
            {
                ParseFault();
                return _errorMessage;
            }
        }

        public Exception Exception
        {
            get
            {
                ParseFault();
                return _exception;
            }
        }

        public System.Net.HttpStatusCode StatusCode { get; private set; }

        void ParseFault()
        {
            if (_exception != null)
                return;
            switch (_messageFormat)
            {
                case VRHttpMessageFormat.ApplicationJSON:
                    var jsonFault = Serializer.Deserialize<JSONFault>(this.StringResponse);
                    if (jsonFault.Message != null && jsonFault.ExceptionMessage != null)
                    {
                        _errorMessage = jsonFault.Message;
                        _exception = new Exception(string.Format("{0}: {1}. Stack Trace: {2}", jsonFault.Message, jsonFault.ExceptionMessage, jsonFault.StackTrace));
                    }
                    break;
                case VRHttpMessageFormat.ApplicationXML:
                case VRHttpMessageFormat.TextXML:
                    var soapFault = new VRXmlSerializer().Deserialize<Fault>(this.StringResponse);
                    if (soapFault.faultcode != null && soapFault.faultstring != null)
                    {
                        _errorMessage = soapFault.faultstring;
                        _exception = new Exception(string.Format("{0}: {1}", soapFault.faultcode, soapFault.faultstring));
                    }
                    break;
                default: throw new NotSupportedException(String.Format("_messageFormat: '{0}'", _messageFormat.ToString()));
            }
            if (_exception == null)
                _exception = new Exception(this.StringResponse);

            if (_errorMessage == null)
                _errorMessage = _exception.Message;
        }

        /// <summary>
        /// SOAP Fault
        /// </summary>
        private class Fault
        {
            public string faultcode { get; set; }

            public string faultstring { get; set; }
        }

        private class JSONFault
        {
            public string Message { get; set; }

            public string ExceptionMessage { get; set; }

            public string StackTrace { get; set; }
        }
    }
}

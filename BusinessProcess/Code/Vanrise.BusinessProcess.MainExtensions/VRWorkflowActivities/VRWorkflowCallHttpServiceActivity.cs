using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities
{
    public class VRWorkflowCallHttpServiceActivity : VRWorkflowActivitySettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("A9C74099-C36E-45E6-8318-44C7B9A2B381"); }
        }

        public override string Editor { get { throw new NotImplementedException(); } }

        public override string Title { get { throw new NotImplementedException(); } }

        public Guid ConnectionId { get; set; }

        public VRWorkflowCallHttpServiceMethod Method { get; set; }

        public string ActionPath { get; set; }

        public List<VRWorkflowCallHttpServiceURLParameter> URLParameters { get; set; }

        public List<VRWorkflowCallHttpServiceHeader> Headers { get; set; }

        public string BuildBodyLogic { get; set; }

        public VRWorkflowCallHttpServiceMessageFormat MessageFormat { get; set; }

        public string ResponseLogic { get; set; }

        public string ErrorLogic { get; set; }

        public string MaxRetryCount { get; set; }

        public string RetryInterval { get; set; }

        public string IsSucceeded { get; set; }

        public bool ContinueWorkflowIfCallFailed { get; set; }

        public override string GenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            context.AddUsingStatement("using System.Net.Http;");
            StringBuilder nmSpaceCodeBuilder = new StringBuilder(@"                 

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : CodeActivity
                    {
                        protected override void Execute(CodeActivityContext context)
                        {
                            var executionContext = new #CLASSNAME#ExecutionContext(context);
                            executionContext.Execute();
                        }
                    }

                    #BASEEXECUTIONCLASSCODE#                  

                    public class #CLASSNAME#ExecutionContext : #BASEEXECUTIONCLASSNAME#
                    {
                        public #CLASSNAME#ExecutionContext(ActivityContext activityContext) 
                            : base (activityContext)
                        {
                        }

                        public void Execute()
                        {
                            using (var client = new System.Net.Http.HttpClient())
                            {
                                client.BaseAddress = new Uri(""http://localhost:1206"");
                                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(""#MESSAGEFORMATVALUE#""));
                                using (System.Net.Http.HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.#METHOD#, #ACTIONPATH# + ""#URLPARAMETERS#""))
                                {
                                    #HEADERS#
                    
                                    #BODY#
                    
                                    using (var responseTask = client.SendAsync(request))
                                    {
                                        responseTask.Wait();
                                        if(responseTask.Exception == null && responseTask.Result.IsSuccessStatusCode)
                                        {
                                            #SETISSUCCEEDEDTRUE#
                                            OnResponseReceived(new Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowCallHttpServiceResponse(client, responseTask.Result, Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowCallHttpServiceMessageFormat.#MESSAGEFORMAT#));                                
                                        }
                                        else
                                        {
                                            #SETISSUCCEEDEDFALSE#
                                            var fault = new Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowCallHttpServiceFault(client, responseTask, Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowCallHttpServiceMessageFormat.#MESSAGEFORMAT#);
                                            OnError(fault);
                                            #ERRORHANDLING#
                                        }     
                                    }
                                }
                            }
                           
                        }

                        #BUILDBODYMETHOD#
                        
                        void OnResponseReceived(Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowCallHttpServiceResponse response)
                        {
                            #RESPONSELOGIC#
                        }

                        void OnError(Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowCallHttpServiceFault error)
                        {
                            #ERRORLOGIC#
                        }
                    }
                }");


            nmSpaceCodeBuilder.Replace("#ACTIONPATH#", this.ActionPath);
            nmSpaceCodeBuilder.Replace("#METHOD#", this.Method.ToString());

            if (this.URLParameters != null && this.URLParameters.Count > 0)
            {
                StringBuilder urlParametersBuilder = new StringBuilder();
                int index = 0;
                foreach (var prm in this.URLParameters)
                {
                    if (index == 0)
                        urlParametersBuilder.Append("?");
                    else
                        urlParametersBuilder.Append(",");
                    index++;
                    urlParametersBuilder.Append(String.Concat("", prm.Name, "=\" + "));
                    urlParametersBuilder.Append(prm.Value);
                    urlParametersBuilder.Append(" + \"");
                }
                nmSpaceCodeBuilder.Replace("#URLPARAMETERS#", urlParametersBuilder.ToString());
            }
            else
            {
                nmSpaceCodeBuilder.Replace("#URLPARAMETERS#", "");
            }

            if (this.Headers != null)
            {
                StringBuilder headersBuilder = new StringBuilder();
                foreach (var header in this.Headers)
                {
                    headersBuilder.AppendLine(String.Concat("request.AddHeader(\"", header.Key, "\", ", header.Value, ");"));
                }
                nmSpaceCodeBuilder.Replace("#HEADERS#", headersBuilder.ToString());
            }
            else
            {
                nmSpaceCodeBuilder.Replace("#HEADERS#", "");
            }

            string messageFormatValue = Utilities.GetEnumAttribute<VRWorkflowCallHttpServiceMessageFormat, VRWorkflowCallHttpServiceMessageFormatAttribute>(this.MessageFormat).Value;

            nmSpaceCodeBuilder.Replace("#MESSAGEFORMAT#", this.MessageFormat.ToString());
            nmSpaceCodeBuilder.Replace("#MESSAGEFORMATVALUE#", messageFormatValue);

            if (this.BuildBodyLogic != null)
            {
                nmSpaceCodeBuilder.Replace("#BODY#", String.Concat("request.Content = new System.Net.Http.StringContent(GetBody(), System.Text.Encoding.UTF8, \"", messageFormatValue, "\");"));
                nmSpaceCodeBuilder.Replace("#BUILDBODYMETHOD#", String.Concat(@"string GetBody() 
                                            {
                                            ", this.BuildBodyLogic, @"
                                            }"));
            }
            else
            {
                nmSpaceCodeBuilder.Replace("#BODY#", "");
                nmSpaceCodeBuilder.Replace("#BUILDBODYMETHOD#", "");
            }

            if (this.ResponseLogic != null)
            {
                nmSpaceCodeBuilder.Replace("#RESPONSELOGIC#", this.ResponseLogic);
            }
            else
            {
                nmSpaceCodeBuilder.Replace("#RESPONSELOGIC#", "");
            }

            if (!string.IsNullOrEmpty(this.ErrorLogic))
            {
                nmSpaceCodeBuilder.Replace("#ERRORLOGIC#", this.ErrorLogic);
            }
            else
            {
                nmSpaceCodeBuilder.Replace("#ERRORLOGIC#", "");
            }

            if (!String.IsNullOrEmpty(this.IsSucceeded))
            {
                nmSpaceCodeBuilder.Replace("#SETISSUCCEEDEDTRUE#", string.Concat(this.IsSucceeded, " = true;"));
                nmSpaceCodeBuilder.Replace("#SETISSUCCEEDEDFALSE#", string.Concat(this.IsSucceeded, " = false;"));
            }
            else
            {
                nmSpaceCodeBuilder.Replace("#SETISSUCCEEDEDTRUE#", "");
                nmSpaceCodeBuilder.Replace("#SETISSUCCEEDEDFALSE#", "");
            }

            if (!this.ContinueWorkflowIfCallFailed)
            {
                nmSpaceCodeBuilder.Replace("#ERRORHANDLING#", "throw fault.Exception;");
            }
            else
            {
                nmSpaceCodeBuilder.Replace("#ERRORHANDLING#", "");
            }

            string baseExecutionContextClassName = "CallServiceActivity_BaseExecutionContext";
            string baseExecutionContextClassCode = CodeGenerationHelper.GenerateBaseExecutionClass(context, baseExecutionContextClassName);
            nmSpaceCodeBuilder.Replace("#BASEEXECUTIONCLASSCODE#", baseExecutionContextClassCode);
            nmSpaceCodeBuilder.Replace("#BASEEXECUTIONCLASSNAME#", baseExecutionContextClassName);
            string codeNamespace = context.GenerateUniqueNamespace("CallServiceActivity");
            string className = "CallServiceActivity";
            string fullTypeName = string.Concat(codeNamespace, ".", className);

            nmSpaceCodeBuilder.Replace("#NAMESPACE#", codeNamespace);
            nmSpaceCodeBuilder.Replace("#CLASSNAME#", className);
            context.AddFullNamespaceCode(nmSpaceCodeBuilder.ToString());

            return string.Format("new {0}()", fullTypeName);
        }
    }

    public enum VRWorkflowCallHttpServiceMethod
    {
        GET = 0,
        POST = 1
    }

    public enum VRWorkflowCallHttpServiceMessageFormat
    {
        [VRWorkflowCallHttpServiceMessageFormat("application/json")]
        ApplicationJSON = 0,
        [VRWorkflowCallHttpServiceMessageFormat("application/xml")]
        ApplicationXML = 1,
        [VRWorkflowCallHttpServiceMessageFormat("text/xml")]
        TextXML = 2
    }

    public class VRWorkflowCallHttpServiceMessageFormatAttribute : Attribute
    {
        public VRWorkflowCallHttpServiceMessageFormatAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; set; }
    }

    public class VRWorkflowCallHttpServiceURLParameter
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }

    public class VRWorkflowCallHttpServiceHeader
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }

    public class VRWorkflowCallHttpServiceResponse
    {
        HttpClient _client;
        HttpResponseMessage _response;
        VRWorkflowCallHttpServiceMessageFormat _messageFormat;

        public VRWorkflowCallHttpServiceResponse(HttpClient client, HttpResponseMessage response, VRWorkflowCallHttpServiceMessageFormat messageFormat)
        {
            _client = client;
            _response = response;
            _messageFormat = messageFormat;
        }

        public string StringResponse
        {
            get
            {
                return GetResponseAsString();
            }
        }

        public System.Net.HttpStatusCode StatusCode
        {
            get
            {
                return _response.StatusCode;
            }
        }

        public T DeserializeResponse<T>()
        {
            string responseAsString = StringResponse;
            if (String.IsNullOrEmpty(responseAsString))
            {
                return default(T);
            }
            else
            {
                switch (_messageFormat)
                {
                    case VRWorkflowCallHttpServiceMessageFormat.ApplicationJSON:
                        return Common.Serializer.Deserialize<T>(responseAsString);
                    case VRWorkflowCallHttpServiceMessageFormat.ApplicationXML:
                    case VRWorkflowCallHttpServiceMessageFormat.TextXML:
                        return new VRXmlSerializer().Deserialize<T>(responseAsString);
                    default: throw new NotSupportedException(String.Format("_messageFormat: '{0}'", _messageFormat.ToString()));
                }

            }
        }

        string _responseAsString;
        string GetResponseAsString()
        {
            _responseAsString = _response.Content.ReadAsStringAsync().Result;
            return _responseAsString;
        }
    }

    public class VRWorkflowCallHttpServiceFault
    {
        HttpClient _client;
        Task<HttpResponseMessage> _responseTask;
        VRWorkflowCallHttpServiceMessageFormat _messageFormat;

        private Fault _soapFault;
        private JSONFault _jsonFault;

        public VRWorkflowCallHttpServiceFault(HttpClient client, Task<HttpResponseMessage> responseTask, VRWorkflowCallHttpServiceMessageFormat messageFormat)
        {
            _client = client;
            _responseTask = responseTask;
            _messageFormat = messageFormat;
        }

        public string StringResponse
        {
            get
            {
                return GetResponseAsString();
            }
        }

        string _responseAsString;
        string GetResponseAsString()
        {
            _responseAsString = _responseTask.Result.Content.ReadAsStringAsync().Result;
            return _responseAsString;
        }

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

        public System.Net.HttpStatusCode StatusCode
        {
            get
            {
                return _responseTask.Result.StatusCode;
            }
        }

        string _errorMessage;
        Exception _exception;

        void ParseFault()
        {
            if (_exception != null)
                return;
            if (_responseTask.Exception != null)
            {
                _exception = _responseTask.Exception;
                _errorMessage = _exception.Message;
            }
            else
            {
                switch (_messageFormat)
                {
                    case VRWorkflowCallHttpServiceMessageFormat.ApplicationJSON:
                        _jsonFault = Serializer.Deserialize<JSONFault>(GetResponseAsString());
                        if (_jsonFault.Message != null && _jsonFault.ExceptionMessage != null)
                        {
                            _errorMessage = _jsonFault.Message;
                            _exception = new Exception(string.Format("{0}: {1}. Stack Trace: {2}", _jsonFault.Message, _jsonFault.ExceptionMessage, _jsonFault.StackTrace));
                        }
                        break;
                    case VRWorkflowCallHttpServiceMessageFormat.ApplicationXML:
                    case VRWorkflowCallHttpServiceMessageFormat.TextXML:
                        _soapFault = new VRXmlSerializer().Deserialize<Fault>(GetResponseAsString());
                        if (_soapFault.faultcode != null && _soapFault.faultstring != null)
                        {
                            _errorMessage = _soapFault.faultstring;
                            _exception = new Exception(string.Format("{0}: {1}", _soapFault.faultcode, _soapFault.faultstring));
                        }
                        break;
                    default: throw new NotSupportedException(String.Format("_messageFormat: '{0}'", _messageFormat.ToString()));
                }
                if (_exception == null)
                    _exception = new Exception(GetResponseAsString());
            }
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities
{
    public class VRWorkflowCallHttpServiceActivity : VRWorkflowActivitySettings
    {
        public override Guid ConfigId { get { return new Guid("A9C74099-C36E-45E6-8318-44C7B9A2B381"); } }

        public override string Editor { get { return "businessprocess-vr-workflowactivity-callhttpservice"; } }

        public override string Title { get { return "Call Http Service"; } }

        public string ServiceName { get; set; }

        public Guid ConnectionId { get; set; }

        public VRWorkflowCallHttpServiceMethod Method { get; set; }

        public string ActionPath { get; set; }

        public List<VRWorkflowCallHttpServiceURLParameter> URLParameters { get; set; }

        public List<VRWorkflowCallHttpServiceHeader> Headers { get; set; }

        public string BuildBodyLogic { get; set; }

        public VRWorkflowCallHttpServiceMessageFormat MessageFormat { get; set; }

        public string ResponseLogic { get; set; }

        public string ErrorLogic { get; set; }

        public string IsSucceeded { get; set; }

        public bool ContinueWorkflowIfCallFailed { get; set; }

        public VRWorkflowCallHttpRetrySettings RetrySettings { get; set; }

        protected override string InternalGenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            var httpClient = new System.Net.Http.HttpClient();
            context.AddUsingStatement("using System.Net.Http;");
            StringBuilder nmSpaceCodeBuilder = new StringBuilder(@"                 

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : NativeActivity
                    {
                        protected override void Execute(NativeActivityContext context)
                        {
                            var executionContext = new #CLASSNAME#ExecutionContext(context);
                            executionContext.Execute(OnDelayCompleted, 0);
                        }

                        private void OnDelayCompleted(NativeActivityContext context, Bookmark bookmark, object value)
                        {
                            var httpDelayInput = value as Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRHttpDelayInput;
                            if (httpDelayInput == null)
                                throw new ArgumentNullException(""httpDelayInput"");

                            var executionContext = new #CLASSNAME#ExecutionContext(context);
                            executionContext.Execute(OnDelayCompleted, httpDelayInput.RetryCount);
                        }

                        protected override bool CanInduceIdle
                        {
                            get
                            {
                                return true;
                            }
                        }
                    }

                    #BASEEXECUTIONCLASSCODE#                  

                    public class #CLASSNAME#ExecutionContext : #BASEEXECUTIONCLASSNAME#
                    {
                        static Vanrise.Common.Business.VRConnectionManager s_connectionManager = new Vanrise.Common.Business.VRConnectionManager();
                        static Vanrise.BusinessProcess.Business.BPTimeSubscriptionManager s_bpTimeSubscriptionManager = new Vanrise.BusinessProcess.Business.BPTimeSubscriptionManager();
                        NativeActivityContext _activityContext;

                        public #CLASSNAME#ExecutionContext(NativeActivityContext activityContext) 
                            : base (activityContext)
                        {
                            _activityContext = activityContext;
                        }

                        public void Execute(BookmarkCallback onDelayCompleted, int retryCount)
                        {
                            Guid connectionId = new Guid(""#CONNECTIONID#"");
                            var vrConnection = s_connectionManager.GetVRConnection(connectionId);
                            vrConnection.ThrowIfNull(""vrConnection"", connectionId);
                            var vrHttpConnection = vrConnection.Settings.CastWithValidate<Vanrise.Common.Business.VRHttpConnection>(""vrConnection.Settings"", connectionId);
                        
                            List<TimeSpan> delays = GetDelays();
                            bool throwIfError = (delays == null || delays.Count == retryCount) && #THROWIFERROR#;
                            #ASSIGNISSUCCEEDED#vrHttpConnection.TrySendRequest(#ACTIONPATH#, Vanrise.Entities.VRHttpMethod.#HTTPMETHOD#, Vanrise.Entities.VRHttpMessageFormat.#MESSAGEFORMAT#, #URLPARAMETERS#,
                            #HEADERS#, #BODY#, (response) => OnResponseReceived(new Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowCallHttpServiceResponse(response)),
                            throwIfError, (error) => OnError(new Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowCallHttpServiceFault(error), onDelayCompleted, retryCount));
                        }

                        #BUILDURLPARAMETERSMETHOD#

                        #BUILDHEADERSMETHOD#

                        #BUILDBODYMETHOD#
                        
                        List<TimeSpan> GetDelays()
                        {
                            Guid connectionId = new Guid(""#CONNECTIONID#"");
                            var vrConnection = s_connectionManager.GetVRConnection(connectionId);
                            vrConnection.ThrowIfNull(""vrConnection"", connectionId);
                            var vrHttpConnection = vrConnection.Settings.CastWithValidate<Vanrise.Common.Business.VRHttpConnection>(""vrConnection.Settings"", connectionId);

                            Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowCallHttpRetrySettings retrySettings = Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowCallHttpRetrySettings.#RetrySettings#;
                            List<TimeSpan> delays = new List<TimeSpan>();
                            switch (retrySettings)
                            {
                                case Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowCallHttpRetrySettings.NoRetry: break;
                                case Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowCallHttpRetrySettings.RetryAsPerConnection:

                                    List<Vanrise.Common.Business.VRWorkflowRetrySettings> vrWorkflowRetrySettings = vrHttpConnection.WorkflowRetrySettings;
                                    if (vrWorkflowRetrySettings == null || vrWorkflowRetrySettings.Count == 0)
                                        break;

                                    foreach (Vanrise.Common.Business.VRWorkflowRetrySettings workflowRetrySettings in vrWorkflowRetrySettings)
                                        delays.AddRange(Enumerable.Repeat(workflowRetrySettings.RetryInterval, workflowRetrySettings.MaxRetryCount));

                                    break;
                            }
                            return delays.Count > 0 ? delays : null;
                        }

                        void OnResponseReceived(Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowCallHttpServiceResponse response)
                        {
                            #RESPONSELOGIC#
                        }

                        void OnError(Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowCallHttpServiceFault error, BookmarkCallback onDelayCompleted, int retryCount)
                        {
                            #ERRORLOGIC#
                            
                            List<TimeSpan> delays = GetDelays();
                            if(delays == null || delays.Count <= retryCount)
                                return;

                            long processInstanceId = _activityContext.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
                            TimeSpan delay = delays[retryCount];
                            
                            retryCount++;
                            string bookmarkName = Vanrise.BusinessProcess.Business.BPTimeSubscriptionManager.GetWFBookmark(processInstanceId);
                            var input = new Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRHttpDelayInput(){ NoMoreRanges = delays.Count == retryCount, RetryCount = retryCount };
                            new Vanrise.BusinessProcess.Business.BPTimeSubscriptionManager().InsertBPTimeSubscription(processInstanceId, bookmarkName, delay, input);
                            _activityContext.CreateBookmark(bookmarkName, onDelayCompleted);
                        }
                    }
                }");

            nmSpaceCodeBuilder.Replace("#CONNECTIONID#", this.ConnectionId.ToString());
            nmSpaceCodeBuilder.Replace("#ACTIONPATH#", this.ActionPath);
            nmSpaceCodeBuilder.Replace("#HTTPMETHOD#", this.Method.ToString());
            nmSpaceCodeBuilder.Replace("#RetrySettings#", this.RetrySettings.ToString());

            if (this.URLParameters != null && this.URLParameters.Count > 0)
            {
                StringBuilder urlParametersBuilder = new StringBuilder();
                foreach (var prm in this.URLParameters)
                {
                    urlParametersBuilder.AppendLine(String.Concat("parameters.Add(\"", prm.Name, "\", ", prm.Value, ");"));
                }
                nmSpaceCodeBuilder.Replace("#BUILDURLPARAMETERSMETHOD#", String.Concat(@"Dictionary<string, string> GetUrlParameters() 
                                            {
                                                Dictionary<string, string> parameters = new Dictionary<string, string>();
                                                ", urlParametersBuilder.ToString(), @"
                                                return parameters;
                                            }"));
                nmSpaceCodeBuilder.Replace("#URLPARAMETERS#", "GetUrlParameters()");
            }
            else
            {
                nmSpaceCodeBuilder.Replace("#BUILDURLPARAMETERSMETHOD#", "");
                nmSpaceCodeBuilder.Replace("#URLPARAMETERS#", "null");
            }

            if (this.Headers != null && this.Headers.Count > 0)
            {
                StringBuilder headersBuilder = new StringBuilder();
                foreach (var header in this.Headers)
                {
                    headersBuilder.AppendLine(String.Concat("headers.Add(\"", header.Key, "\", ", header.Value, ");"));
                }
                nmSpaceCodeBuilder.Replace("#BUILDHEADERSMETHOD#", String.Concat(@"Dictionary<string, string> GetHeaders() 
                                            {
                                                Dictionary<string, string> headers = new Dictionary<string, string>();
                                                ", headersBuilder.ToString(), @"
                                                return headers;
                                            }"));
                nmSpaceCodeBuilder.Replace("#HEADERS#", "GetHeaders()");
            }
            else
            {
                nmSpaceCodeBuilder.Replace("#BUILDHEADERSMETHOD#", "");
                nmSpaceCodeBuilder.Replace("#HEADERS#", "null");
            }

            nmSpaceCodeBuilder.Replace("#MESSAGEFORMAT#", this.MessageFormat.ToString());

            if (this.BuildBodyLogic != null)
            {
                nmSpaceCodeBuilder.Replace("#BODY#", "GetBody()");
                nmSpaceCodeBuilder.Replace("#BUILDBODYMETHOD#", String.Concat(@"string GetBody() 
                                            {
                                            ", this.BuildBodyLogic, @"
                                            }"));
            }
            else
            {
                nmSpaceCodeBuilder.Replace("#BODY#", "null");
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
                nmSpaceCodeBuilder.Replace("#ASSIGNISSUCCEEDED#", string.Concat(this.IsSucceeded, " = "));
            }
            else
            {
                nmSpaceCodeBuilder.Replace("#ASSIGNISSUCCEEDED#", "");
            }

            nmSpaceCodeBuilder.Replace("#THROWIFERROR#", this.ContinueWorkflowIfCallFailed ? "false" : "true");

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

    public enum VRWorkflowCallHttpRetrySettings
    {
        NoRetry = 0,
        RetryAsPerConnection = 1
    }

    public enum VRWorkflowCallHttpServiceMethod
    {
        Get = 0,
        Post = 1,
        Put = 2,
        Delete = 3
    }

    public enum VRWorkflowCallHttpServiceMessageFormat
    {
        ApplicationJSON = 0,
        ApplicationXML = 1,
        TextXML = 2
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
        VRHttpResponse _vrHttpResponse;

        public VRWorkflowCallHttpServiceResponse(VRHttpResponse vrHttpResponse)
        {
            _vrHttpResponse = vrHttpResponse;
        }

        public string StringResponse
        {
            get
            {
                return _vrHttpResponse.Response;
            }
        }

        public System.Net.HttpStatusCode StatusCode
        {
            get
            {
                return _vrHttpResponse.StatusCode;
            }
        }

        public T DeserializeResponse<T>()
        {
            return _vrHttpResponse.DeserializeResponse<T>();
        }
    }

    public class VRWorkflowCallHttpServiceFault
    {
        VRHttpFault _vrHttpFault;

        public VRWorkflowCallHttpServiceFault(VRHttpFault vrHttpFault)
        {
            _vrHttpFault = vrHttpFault;
        }

        public string StringResponse
        {
            get
            {
                return _vrHttpFault.StringResponse;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return _vrHttpFault.ErrorMessage;
            }
        }

        public Exception Exception
        {
            get
            {
                return _vrHttpFault.Exception;
            }
        }

        public System.Net.HttpStatusCode StatusCode
        {
            get
            {
                return _vrHttpFault.StatusCode;
            }
        }
    }

    public class VRHttpDelayInput : BPTimeSubscriptionPayload
    {
        public bool NoMoreRanges { get; set; }

        public int RetryCount { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Entities;

namespace SOM.ST.Business
{
    public class FailureBPSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override void OnBPExecutionCompleted(Vanrise.BusinessProcess.Entities.IBPDefinitionBPExecutionCompletedContext context)
        {
            var commonInput = context.BPInstance.InputArgument as CommonInput;

            if (context.BPInstance.Status != BPInstanceStatus.Completed)
            {
                var path = "/0/rest/StRequestManager/SetRequestFailed";
                string body = null;

                var urlParameters = new Dictionary<string, string>();
                urlParameters.Add("requestId", commonInput.CommonInputArgument.RequestId);

                var headers = new Dictionary<string, string>();
                var connection = new Vanrise.Common.Business.VRHttpConnection()
                {
                    BaseURL = "http://192.168.25.9:7121",
                    Headers = new List<Vanrise.Common.Business.VRHttpHeader>(),
                    Interceptor = new SOM.ST.Business.BPMOnlineInterceptor
                    {
                        UserName = "Supervisor",
                        Password = "Supervisor",
                        AuthenticationServiceURI = "/ServiceModel/AuthService.svc/Login"
                    }
                };
                connection.TrySendRequest(path, VRHttpMethod.Get, VRHttpMessageFormat.ApplicationJSON, urlParameters, headers, body, null, true, null);
            }
            base.OnBPExecutionCompleted(context);
        }
    }

    public class CommonInput : BaseProcessInputArgument
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public override string GetTitle()
        {
            return this.ProcessName;
        }
    }

    public class CommonInputArgument
    {
        public string ContactId { get; set; }
        public string AccountId { get; set; }
        public string ContractId { get; set; }
        public string CustomerId { get; set; }
        public string RequestId { get; set; }
        public string SwitchId { get; set; }
    }
}

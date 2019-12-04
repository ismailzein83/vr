using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace SOM.ST.Business
{
    public class FailureBPSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        static Guid bpmConnection = new Guid("E9159CBF-974F-4F0B-9D14-BC2414736DD4"); 
        public override void OnBPExecutionCompleted(Vanrise.BusinessProcess.Entities.IBPDefinitionBPExecutionCompletedContext context)
        {
            context.ThrowIfNull("context");
            context.BPInstance.ThrowIfNull("context.BPInstance");
            context.BPInstance.InputArgument.ThrowIfNull("context.BPInstance.InputArgument");

            dynamic inputDynamic = context.BPInstance.InputArgument;
            var requestId = (string)inputDynamic.CommonInputArgument.RequestId;
            requestId.ThrowIfNull("CommonInputArgument.RequestId");

            if (context.BPInstance.Status != BPInstanceStatus.Completed)
            {
                var path = "/0/rest/StRequestManager/SetRequestFailed";
                string body = null;
                var urlParameters = new Dictionary<string, string>();
                urlParameters.Add("requestId", requestId);

                var vrConnection = new VRConnectionManager().GetVRConnection<VRHttpConnection>(bpmConnection);
                VRHttpConnection connectionSettings = vrConnection.Settings as VRHttpConnection;
                var headers = new Dictionary<string, string>();
                connectionSettings.TrySendRequest(path, VRHttpMethod.Get, VRHttpMessageFormat.ApplicationJSON, urlParameters, headers, body, null, true, null);
            }
            base.OnBPExecutionCompleted(context);
        }
    }

    public class CommonInputArgument
    {
        public string RequestId { get; set; }
    }
}


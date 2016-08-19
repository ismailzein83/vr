using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime
{
    internal class PingRunningProcessServiceRequest : InterRuntimeServiceRequest<PingRunningProcessServiceResponse>
    {
        public int RunningProcessId { get; set; }

        public override PingRunningProcessServiceResponse Execute()
        {
            PingRunningProcessServiceResponse response = new PingRunningProcessServiceResponse();
            if (RunningProcessManager.CurrentProcess.ProcessId != this.RunningProcessId)//this occurs if same Service URL was used by previously killed running process
                response.Result = PingRunningProcessResult.InvalidProcessId;
            else
                response.Result = PingRunningProcessResult.Succeeded;
            return response;
        }
    }

    public enum PingRunningProcessResult { Succeeded, InvalidProcessId }
    internal class PingRunningProcessServiceResponse
    {
        public PingRunningProcessResult Result { get; set; }
    }
}

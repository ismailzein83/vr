using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime
{
    [ServiceContract(Namespace = "http://runtime.vanrise.com/IRuntimeManagerWCFService")]
    internal interface IRuntimeManagerWCFService
    {
        [OperationContract]
        HeartBeatResponse UpdateHeartBeat(HeartBeatRequest request);
    }

    public class HeartBeatRequest
    {
        public int RunningProcessId { get; set; }
    }

    public enum HeartBeatResult { Succeeded, ProcessNotExists }

    public class HeartBeatResponse
    {
        public HeartBeatResult Result { get; set; }
    }
}

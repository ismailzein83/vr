using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime
{
    [ServiceContract(Namespace = "http://runtime.vanrise.com/IRuntimeManagerWCFService")]
    internal interface IRuntimeManagerWCFService
    {
        [OperationContract]
        HeartBeatResponse UpdateHeartBeat(HeartBeatRequest request);

        [OperationContract]
        bool TryLockRuntimeService(string serviceTypeUniqueName, int runtimeProcessId);

        [OperationContract]
        GetServiceProcessIdResponse TryGetServiceProcessId(GetServiceProcessIdRequest request);

        [OperationContract]
        bool TryLock(TransactionLockItem lockItem, int maxAllowedConcurrency);

        [OperationContract]
        void UnLock(TransactionLockItem lockItem);
    }

    public class HeartBeatRequest
    {
        public int RunningProcessId { get; set; }

        public List<TransactionLockItem> FreezedTransactionLocks { get; set; }
    }

    public enum HeartBeatResult { Succeeded, ProcessNotExists }

    public class HeartBeatResponse
    {
        public HeartBeatResult Result { get; set; }
    }

    public class LockRuntimeServiceRequest
    {
        public string ServiceTypeUniqueName { get; set; }

        public int RuntimeProcessId { get; set; }
    }

    public enum LockRuntimeServiceResult { Succeeded, Failed }

    public class LockRuntimeServiceResponse
    {
        public LockRuntimeServiceResult Result { get; set; }
    }

    public class GetServiceProcessIdRequest
    {
        public string ServiceTypeUniqueName { get; set; }
    }

    public class GetServiceProcessIdResponse
    {
        public int? RuntimeProcessId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime
{
   internal class RuntimeManagerWCFService : IRuntimeManagerWCFService
    {
        public HeartBeatResponse UpdateHeartBeat(HeartBeatRequest request)
        {
            HeartBeatResponse response = new HeartBeatResponse();
            if (ProcessHeartBeatManager.Current.UpdateProcessHB(request.RunningProcessId))
                response.Result = HeartBeatResult.Succeeded;
            else
                response.Result = HeartBeatResult.ProcessNotExists;
            if(request.FreezedTransactionLocks != null)
            {
                foreach(var transactionLockItem in request.FreezedTransactionLocks)
                {
                    UnLock(transactionLockItem);
                }
            }
            //Console.WriteLine("{0: HH:mm:ss} heartbeat received from process {1}. Result is {2}", DateTime.Now, request.RunningProcessId, response.Result);
            return response;
        }

        static Dictionary<string, int> s_runtimeServiceTypeProcessIds = new Dictionary<string, int>();
        static RuntimeServiceInstanceManager s_runtimeServiceInstanceManager = new RuntimeServiceInstanceManager();

        public bool TryLockRuntimeService(string serviceTypeUniqueName, int runtimeProcessId)
        {
            int currentLockedProcessId;
            if (s_runtimeServiceTypeProcessIds.TryGetValue(serviceTypeUniqueName, out currentLockedProcessId) && currentLockedProcessId == runtimeProcessId)
            {
                return true;
            }
            else
            {
                HashSet<int> runningServicesProcessIds = new HashSet<int>(s_runtimeServiceInstanceManager.GetServices(serviceTypeUniqueName).Select(itm => itm.ProcessId));
                lock (s_runtimeServiceTypeProcessIds)
                {
                    if (s_runtimeServiceTypeProcessIds.TryGetValue(serviceTypeUniqueName, out currentLockedProcessId))
                    {
                        if (currentLockedProcessId == runtimeProcessId)
                            return true;
                        else if (runningServicesProcessIds.Contains(currentLockedProcessId))
                            return false;
                        else
                        {
                            s_runtimeServiceTypeProcessIds[serviceTypeUniqueName] = runtimeProcessId;
                            return true;
                        }
                    }
                    else
                    {
                        s_runtimeServiceTypeProcessIds.Add(serviceTypeUniqueName, runtimeProcessId);
                        return true;
                    }
                }
            }
        }

        public GetServiceProcessIdResponse TryGetServiceProcessId(GetServiceProcessIdRequest request)
        {
             int currentLockedProcessId;
             if (s_runtimeServiceTypeProcessIds.TryGetValue(request.ServiceTypeUniqueName, out currentLockedProcessId))
             {
                 return new GetServiceProcessIdResponse
                 {
                     RuntimeProcessId = currentLockedProcessId
                 };
             }
             else
                 return null;
        }


        public bool TryLock(Entities.TransactionLockItem lockItem, int maxAllowedConcurrency)
        {
            TransactionLockHandler transactionLockHandler = TransactionLockHandler.Current;
            if (transactionLockHandler == null)
                throw new NullReferenceException("TransactionLockHandler.Current");
            return transactionLockHandler.TryLock(lockItem, maxAllowedConcurrency);
        }

        public void UnLock(Entities.TransactionLockItem lockItem)
        {
            TransactionLockHandler transactionLockHandler = TransactionLockHandler.Current;
            if (transactionLockHandler == null)
                throw new NullReferenceException("TransactionLockHandler.Current");
            transactionLockHandler.UnLock(lockItem);
        }
    }
}

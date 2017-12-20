using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime
{
   internal class RuntimeManagerWCFService : IRuntimeManagerWCFService
    {
       public bool IsThisCurrentRuntimeManager()
       {
           return RuntimeManager.Current != null && RuntimeManager.Current._isCurrentRuntimeManagerReady;
       }

        public void UnlockFreezedTransactions(List<TransactionLockItem> freezedTransactionLocks)
        {
            if (freezedTransactionLocks != null)
            {
                foreach (var transactionLockItem in freezedTransactionLocks)
                {
                    UnLock(transactionLockItem);
                }
            }
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


        public string RegisterRunningProcess(string serializedInput)
        {
            var output = RuntimeManager.Current.RegisterRunningProcess(Serializer.Deserialize<RunningProcessRegistrationInput>(serializedInput));
            return Serializer.Serialize(output);
        }

       public bool IsRunningProcessStillRegistered(int runningProcessId)
        {
            return RuntimeManager.Current.IsRunningProcessStillRegistered(runningProcessId);
        }
    }
}

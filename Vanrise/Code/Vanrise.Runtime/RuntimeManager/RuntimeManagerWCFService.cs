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
        public void UnlockFreezedTransactions(List<TransactionLockItem> freezedTransactionLocks)
        {
            ValidateCurrentIsThePrimaryManager();

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
            ValidateCurrentIsThePrimaryManager();

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
            ValidateCurrentIsThePrimaryManager();

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


        public bool TryLock(Entities.TransactionLockItem lockItem)
        {
            ValidateCurrentIsThePrimaryManager();

            TransactionLockHandler transactionLockHandler = TransactionLockHandler.Current;
            if (transactionLockHandler == null)
                throw new NullReferenceException("TransactionLockHandler.Current");

            return transactionLockHandler.TryLock(lockItem);
        }

        void ValidateCurrentIsThePrimaryManager()
        {
            if (!RuntimeManager.Current.IsCurrentTheValidPrimaryManager())
                throw new Exception("Current Node is not the Primary Node");
        }

        public void UnLock(Entities.TransactionLockItem lockItem)
        {
            ValidateCurrentIsThePrimaryManager();

            TransactionLockHandler transactionLockHandler = TransactionLockHandler.Current;
            if (transactionLockHandler == null)
                throw new NullReferenceException("TransactionLockHandler.Current");

            transactionLockHandler.UnLock(lockItem);
        }


        public string RegisterRunningProcess(string serializedInput)
        {
            var output = RuntimeManager.Current.RegisterRunningProcessRequestReceived(Serializer.Deserialize<RunningProcessRegistrationInput>(serializedInput));
            return Serializer.Serialize(output);
        }

        public bool IsThisRuntimeNodeInstance(Guid runtimeNodeId, Guid instanceId)
        {
            return RuntimeManager.Current.IsThisRuntimeNodeInstanceRequestReceived(runtimeNodeId, instanceId);
        }
    }
}
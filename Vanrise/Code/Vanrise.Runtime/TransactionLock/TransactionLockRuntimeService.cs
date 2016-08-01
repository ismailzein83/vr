using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Runtime.Data;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime
{
    public class TransactionLockRuntimeService : RuntimeService
    {
        static LocalTransactionLockHandler s_transactionLockHander;
        internal static LocalTransactionLockHandler TransactionLockHander
        {
            get
            {
                return s_transactionLockHander;
            }
        }

        ILockServiceDataManager _dataManager = RuntimeDataManagerFactory.GetDataManager<ILockServiceDataManager>();


        protected override void Execute()
        {
            if (s_transactionLockHander == null)
                TryHandleLocking();
            if (s_transactionLockHander == null)
                return;

            UpdateLockingDetailsIfNeeded();
            if (s_transactionLockHander == null)
                return;
            CheckUnAvailableLockingThreads();
        }

        ServiceHost _serviceHost;
        private void TryHandleLocking()
        {
            int currentRuntimeProcessId = RunningProcessManager.CurrentProcess.ProcessId;
            IEnumerable<int> runningRuntimeProcessesIds = new RunningProcessManager().GetCachedRunningProcesses().Select(itm => itm.ProcessId);

            TransactionLockingDetails lockingDetails;
            if (_dataManager.TryLock(currentRuntimeProcessId, runningRuntimeProcessesIds, out lockingDetails))
            {
                string serviceURL;
                _serviceHost = ServiceHostManager.Current.CreateAndOpenTCPServiceHost(typeof(TransactionLockWCFService), typeof(ITransactionLockWCFService), OnServiceHostCreated, OnServiceHostRemoved, out serviceURL);
                s_transactionLockHander = new LocalTransactionLockHandler(lockingDetails != null ? lockingDetails.LockedTransactions : null);
                _dataManager.UpdateServiceURL(serviceURL);
            }
        }

        DateTime _lastLockingDetailsUpdateTime;
        private void UpdateLockingDetailsIfNeeded()
        {
            if (_lastLockingDetailsUpdateTime < s_transactionLockHander._lastUpdateTime || (DateTime.Now - _lastLockingDetailsUpdateTime).TotalMinutes > 1)
            {
                DateTime previousUpdateTime = _lastLockingDetailsUpdateTime;
                _lastLockingDetailsUpdateTime = DateTime.Now;
                var lockingDetails = new TransactionLockingDetails
                {
                    LockedTransactions = CopyLockedTransactions()
                };
                try
                {
                    if (!_dataManager.UpdateLockingDetails(RunningProcessManager.CurrentProcess.ProcessId, lockingDetails))//this means it is locked by other runtime
                    {
                        s_transactionLockHander = null;
                        if(_serviceHost != null)
                        {
                            _serviceHost.Close();
                            OnServiceHostRemoved(_serviceHost);                            
                            _serviceHost = null;
                        }
                    }
                }
                catch
                {
                    _lastLockingDetailsUpdateTime = previousUpdateTime;
                    throw;
                }
            }
        }

        private Dictionary<string, Dictionary<Guid, TransactionLockItem>> CopyLockedTransactions()
        {
            Dictionary<string, Dictionary<Guid, TransactionLockItem>> copiedTransactions = new Dictionary<string, Dictionary<Guid, TransactionLockItem>>();
            foreach (var entry in s_transactionLockHander.LockedTransactions)
            {
                Dictionary<Guid, TransactionLockItem> copiedEntry = new Dictionary<Guid, TransactionLockItem>();
                foreach(var lockItemEntry in entry.Value)
                {
                    copiedEntry.Add(lockItemEntry.Key, lockItemEntry.Value);
                }
                copiedTransactions.Add(entry.Key, copiedEntry);
            }
            return copiedTransactions;
        }

        private void CheckUnAvailableLockingThreads()
        {
            s_transactionLockHander.RemoveNonRunningTransactions();
        }

        #region ServiceHost Handling

        void OnServiceHostCreated(ServiceHost serviceHost)
        {
            serviceHost.Opening += serviceHost_Opening;
            serviceHost.Opened += serviceHost_Opened;
            serviceHost.Closing += serviceHost_Closing;
            serviceHost.Closed += serviceHost_Closed;
        }

        void OnServiceHostRemoved(ServiceHost serviceHost)
        {
            serviceHost.Opening -= serviceHost_Opening;
            serviceHost.Opened -= serviceHost_Opened;
            serviceHost.Closing -= serviceHost_Closing;
            serviceHost.Closed -= serviceHost_Closed;
        }

        void serviceHost_Opening(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("TransactionLock WCF Service is opening..");
        }

        void serviceHost_Opened(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("TransactionLock WCF Service opened");
        }

        void serviceHost_Closed(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("TransactionLock WCF Service closed");
        }

        void serviceHost_Closing(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("TransactionLock WCF Service is closing..");
        }

        #endregion
    }
}

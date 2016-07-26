using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vanrise.Runtime.Data;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime
{
    public class TransactionLocker
    {
        #region Singleton

        static TransactionLocker s_Instance = new TransactionLocker();

        public static TransactionLocker Instance
        {
            get
            {
                return s_Instance;
            }
        }

        private TransactionLocker()
        {

        }

        #endregion

        #region static

        static TimeSpan s_retrieveServiceURLInterval;
        static TransactionLocker()
        {
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["TransactionLocker_RetrieveServiceURLInterval"], out s_retrieveServiceURLInterval))
                s_retrieveServiceURLInterval = new TimeSpan(0, 0, 10);
        }

        #endregion

        #region Public Methods

        public bool TryLock(string transactionUniqueName, Action lockAction)
        {
            return TryLock(transactionUniqueName, 1, lockAction);
        }

        public bool TryLock(string transactionUniqueName, int maxAllowedConcurrency, Action lockAction)
        {
            if (lockAction == null)
                throw new ArgumentNullException("lockAction");
            var lockHandler = GetLockHandler();
            if (lockHandler == null)
                return false;
            TransactionLockItem lockItem = CreateLockItem(transactionUniqueName);
            if (lockHandler.TryLock(lockItem, maxAllowedConcurrency))
            {
                try
                {
                    lockAction();
                }
                finally
                {
                    try
                    {
                        lockHandler.UnLock(lockItem);
                    }
                    catch
                    {
                        new RunningProcessManager().SetTransactionLockFreezed(lockItem);
                    }
                }
                return true;
            }
            else
                return false;
        }

        #endregion

        #region Private Methods

        private TransactionLockItem CreateLockItem(string transactionUniqueName)
        {
            TransactionLockItem lockItem = new TransactionLockItem
            {
                LockItemUniqueId = Guid.NewGuid(),
                TransactionUniqueName = transactionUniqueName,
                ProcessId = RunningProcessManager.CurrentProcess.ProcessId
            };
            return lockItem;
        }

        ITransactionLockHandler GetLockHandler()
        {
            ITransactionLockHandler lockHandler = TransactionLockRuntimeService.TransactionLockHander;
            if (lockHandler == null)
                lockHandler = GetWCFClientLockHandler();
            return lockHandler;
        }

        private ITransactionLockHandler GetWCFClientLockHandler()
        {
            string serviceURL;
            if (TryGetServiceURL(out serviceURL))
                return new WCFClientTransactionLockHandler(serviceURL);
            else
                return null;
        }

        static string s_serviceUrl;
        static DateTime s_lastServiceURLRetrievedTime;
        static Object s_ServiceURLRetrievalLockObj = new object();
        private bool TryGetServiceURL(out string serviceURL)
        {
            if((DateTime.Now - s_lastServiceURLRetrievedTime) > s_retrieveServiceURLInterval)
            {
                lock(s_ServiceURLRetrievalLockObj)
                {
                    if ((DateTime.Now - s_lastServiceURLRetrievedTime) > s_retrieveServiceURLInterval)
                    {
                        ILockServiceDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ILockServiceDataManager>();
                        s_serviceUrl = dataManager.GetServiceURL();
                        s_lastServiceURLRetrievedTime = DateTime.Now;
                    }
                }
            }
            serviceURL = s_serviceUrl;
            return !string.IsNullOrEmpty(serviceURL);
        }

        #endregion
    }
}

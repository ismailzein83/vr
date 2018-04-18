using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vanrise.Common;
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

        #region Public Methods

        public bool TryLock(string transactionUniqueName, Action lockAction)
        {
            return TryLock(transactionUniqueName, 1, lockAction);
        }

        public bool TryLock(string transactionUniqueName, int maxAllowedConcurrency, Action lockAction)
        {
            if (lockAction == null)
                throw new ArgumentNullException("lockAction");
            TransactionLockItem lockItem;
            if (TryLock(transactionUniqueName, maxAllowedConcurrency, out lockItem))
            {
                try
                {
                    lockAction();
                    return true;
                }
                finally
                {
                    Unlock(lockItem);
                }
            }
            else
            {
                return false;
            }
        }

        public bool TryLock(string transactionUniqueName, out TransactionLockItem lockItem)
        {
            return TryLock(transactionUniqueName, 1, out lockItem);
        }

        public bool TryLock(string transactionUniqueName, int maxAllowedConcurrency, out TransactionLockItem lockItem)
        {
            lockItem = CreateLockItem(transactionUniqueName);
            bool isLocked = false;
            try
            {
                var lockItem_local = lockItem;
                RuntimeManagerClient.CreateClient((client, primaryNodeRuntimeNodeInstanceId) =>
                {
                    isLocked = client.TryLock(lockItem_local, maxAllowedConcurrency);
                });
                if (!isLocked)
                    lockItem = null;
                return isLocked;
            }
            catch
            {
                if (lockItem != null)
                    Unlock(lockItem);//unlock the transaction in case it is locked but response not returned from the RuntimeManager
                throw;
            }
        }


        public void Unlock(TransactionLockItem lockItem)
        {
            try
            {
                RuntimeManagerClient.CreateClient((client, primaryNodeRuntimeNodeInstanceId) =>
                {
                    client.UnLock(lockItem);
                });
            }
            catch (Exception ex)
            {
                RuntimeHost.Current.SetTransactionLockFreezed(lockItem);
                LoggerFactory.GetExceptionLogger().WriteException(ex);
            }
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
        
        #endregion
    }
}

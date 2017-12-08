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

            TransactionLockItem lockItem = CreateLockItem(transactionUniqueName);
            bool isLocked = false;
            RuntimeManagerClient.CreateClient((client) =>
            {
                isLocked = client.TryLock(lockItem, maxAllowedConcurrency);
            });

            if (isLocked)
            {
                try
                {
                    lockAction();
                }
                finally
                {
                    try
                    {
                        RuntimeManagerClient.CreateClient((client) =>
                        {
                            client.UnLock(lockItem);
                        });
                    }
                    catch(Exception ex)
                    {
                        LoggerFactory.GetExceptionLogger().WriteException(ex);
                        RuntimeHost.SetTransactionLockFreezed(lockItem);
                    }
                }
                return true;
            }
            else
            {
                return false;
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

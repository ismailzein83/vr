using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;
using Vanrise.Common;
using Vanrise.Runtime.Data;

namespace Vanrise.Runtime
{
    internal class TransactionLockHandler
    {
        #region Singleton

        static TransactionLockHandler _current;
        public static TransactionLockHandler Current
        {
            get
            {
                return _current;
            }
        }
        
        private TransactionLockHandler()
        {
            _dataManager = RuntimeDataManagerFactory.GetDataManager<ITransactionLockDataManager>();
            LoadMemoryStore();
        }

        internal static void InitializeCurrent()
        {
            _current = new TransactionLockHandler();
        }

        internal static void RemoveCurrent()
        {
            _current = null;
        }

        TransactionLockMemoryStore _memStore;
        ITransactionLockDataManager _dataManager;

        #endregion

        #region Public Methods

        public bool TryLock(TransactionLockItem lockItem, int maxAllowedConcurrency)
        {
            lock (_memStore)
            {                
                TransactionLocksById existingLocks = _memStore.TransactionLocksByTransName.GetOrCreateItem(lockItem.TransactionUniqueName);
                if (existingLocks.Count >= maxAllowedConcurrency)
                    return false;

                AddTransactionLockToDB(lockItem);
                existingLocks.Add(lockItem.LockItemUniqueId, lockItem);
            }
            return true;
        }

        public void UnLock(TransactionLockItem lockItem)
        {
            DeleteTransactionLockFromDB(lockItem.LockItemUniqueId);
            lock (_memStore)
            {
                TransactionLocksById existingLocks;
                if (_memStore.TransactionLocksByTransName.TryGetValue(lockItem.TransactionUniqueName, out existingLocks))
                {
                    if (existingLocks.ContainsKey(lockItem.LockItemUniqueId))
                        existingLocks.Remove(lockItem.LockItemUniqueId);

                    if (existingLocks.Count == 0)
                    {
                        _memStore.TransactionLocksByTransName.Remove(lockItem.TransactionUniqueName);
                    }
                }
            }
        }

        public void RemoveLocksForNotRunningProcesses(List<int> runningProcessesIds)
        {
            List<TransactionLockItem> locksToDelete = GetLocksForNotRunningProcesses(runningProcessesIds);
            if(locksToDelete != null)
            {
                foreach(var lockItem in locksToDelete)
                {
                    UnLock(lockItem);
                }
            }
        }

        #endregion

        #region Private Methods

        private void LoadMemoryStore()
        {
            List<TransactionLockItem> allTransactionLocks = _dataManager.GetAll();
            _memStore = new TransactionLockMemoryStore { TransactionLocksByTransName = new Dictionary<string, TransactionLocksById>() };
            if(allTransactionLocks != null)
            {
                foreach(var lockItem in allTransactionLocks)
                {
                    _memStore.TransactionLocksByTransName.GetOrCreateItem(lockItem.TransactionUniqueName).Add(lockItem.LockItemUniqueId, lockItem);
                }
            }
        }

        private void AddTransactionLockToDB(TransactionLockItem lockItem)
        {
            _dataManager.Add(lockItem);
        }

        private void DeleteTransactionLockFromDB(Guid lockItemId)
        {
            _dataManager.Delete(lockItemId);
        }

        private List<TransactionLockItem> GetLocksForNotRunningProcesses(List<int> runningProcessesIds)
        {
            return _dataManager.GetLocksForNotRunningProcesses(runningProcessesIds);
        }

        #endregion
    }

    internal class TransactionLockMemoryStore
    {
        public Dictionary<string, TransactionLocksById> TransactionLocksByTransName { get; set; }
    }

    internal class TransactionLocksById : Dictionary<Guid, TransactionLockItem>
    {

    }
}

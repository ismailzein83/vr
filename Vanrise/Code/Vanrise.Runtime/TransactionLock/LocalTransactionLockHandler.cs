using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;
using Vanrise.Common;
using Vanrise.Runtime.Data;

namespace Vanrise.Runtime
{
    public class LocalTransactionLockHandler : ITransactionLockHandler
    {

        Dictionary<string, Dictionary<Guid,TransactionLockItem>> _lockedTransactions;
        internal Dictionary<string, Dictionary<Guid, TransactionLockItem>> LockedTransactions
        {
            get
            {
                return _lockedTransactions;
            }
        }

        internal DateTime _lastUpdateTime;

        public LocalTransactionLockHandler(Dictionary<string, Dictionary<Guid, TransactionLockItem>> lockedTransactions)
        {
            _lockedTransactions = lockedTransactions;
            if (_lockedTransactions == null)
                _lockedTransactions = new Dictionary<string, Dictionary<Guid, TransactionLockItem>>();
        }
        
        public bool TryLock(TransactionLockItem lockItem, int maxAllowedConcurrency)
        {
            bool isLocked = false;
            lock (_lockedTransactions)
            {
                Dictionary<Guid, TransactionLockItem> lockedItems = _lockedTransactions.GetOrCreateItem(lockItem.TransactionUniqueName);
                if (lockedItems.Count < maxAllowedConcurrency)
                {
                    lockedItems.Add(lockItem.LockItemUniqueId, lockItem);
                    isLocked = true;
                }
                _lastUpdateTime = DateTime.Now;
            }
            return isLocked;
        }

        public void UnLock(TransactionLockItem lockItem)
        {
            lock(_lockedTransactions)
            {
                Dictionary<Guid, TransactionLockItem> lockedItems;
                if(_lockedTransactions.TryGetValue(lockItem.TransactionUniqueName, out lockedItems))
                {
                    if (lockedItems.ContainsKey(lockItem.LockItemUniqueId))
                    {
                        lockedItems.Remove(lockItem.LockItemUniqueId);
                        if (lockedItems.Count == 0)
                            _lockedTransactions.Remove(lockItem.TransactionUniqueName);
                    }
                }
                _lastUpdateTime = DateTime.Now;
            }
        }

        internal void RemoveNonRunningTransactions()
        {
            HashSet<int> runningProcessIds = new RunningProcessManager().GetCachedRunningProcesses().Select(itm => itm.ProcessId).ToHashSet();
            long maxFreezedTransactionLocksId;
            HashSet<Guid> freezedTransactionLockItemIds = GetFreezedTransactionLockItemIds(out maxFreezedTransactionLocksId);
            List<string> transactionNamesToRemove = new List<string>();
            bool anyTransactionRemoved = false;
            lock(_lockedTransactions)
            {                
                foreach(var transactionEntry in _lockedTransactions)
                {
                    List<Guid> transactionLockItemIdsToRemove = new List<Guid>();
                    foreach(var transactionLockItem in transactionEntry.Value.Values)
                    {
                        if (!runningProcessIds.Contains(transactionLockItem.ProcessId) || freezedTransactionLockItemIds.Contains(transactionLockItem.LockItemUniqueId))
                            transactionLockItemIdsToRemove.Add(transactionLockItem.LockItemUniqueId);
                    }
                    foreach(var  transactionLockItemId in transactionLockItemIdsToRemove)
                    {
                        transactionEntry.Value.Remove(transactionLockItemId);
                        anyTransactionRemoved = true;
                    }
                    if (transactionEntry.Value.Count == 0)
                        transactionNamesToRemove.Add(transactionEntry.Key);
                }
                
                foreach(var transactionName in transactionNamesToRemove)
                {
                    _lockedTransactions.Remove(transactionName);
                }
            }
            if (maxFreezedTransactionLocksId > 0)
                RuntimeDataManagerFactory.GetDataManager<IFreezedTransactionLockDataManager>().DeleteBeforeId(maxFreezedTransactionLocksId);
            if(anyTransactionRemoved)
                _lastUpdateTime = DateTime.Now;
        }

        private static HashSet<Guid> GetFreezedTransactionLockItemIds(out long maxFreezedTransactionLocksId)
        {
            HashSet<Guid> freezedTransactionLockItemIds = new HashSet<Guid>();
            List<FreezedTransactionLock> freezedTransactionLocks = RuntimeDataManagerFactory.GetDataManager<IFreezedTransactionLockDataManager>().GetFreezedTransactionLocks();
            if (freezedTransactionLocks != null && freezedTransactionLocks.Count > 0)
            {
                foreach (var freezedTransactionLockItemId in freezedTransactionLocks.SelectMany(itm => itm.TransactionLockItemIds))
                {
                    freezedTransactionLockItemIds.Add(freezedTransactionLockItemId);
                }
                maxFreezedTransactionLocksId = freezedTransactionLocks.Max(itm => itm.FreezedTransactionLockId);
            }
            else
                maxFreezedTransactionLocksId = 0;
            return freezedTransactionLockItemIds;
        }
    }
}

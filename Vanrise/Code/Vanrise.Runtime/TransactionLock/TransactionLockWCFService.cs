using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime
{
    public class TransactionLockWCFService : ITransactionLockWCFService
    {
        public bool TryLock(TransactionLockItem lockItem, int maxAllowedConcurrency)
        {
            return TransactionLockRuntimeService.TransactionLockHander.TryLock(lockItem, maxAllowedConcurrency);
        }

        public void UnLock(TransactionLockItem lockItem)
        {
            TransactionLockRuntimeService.TransactionLockHander.UnLock(lockItem);
        }
    }
}

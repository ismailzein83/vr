using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime
{
    internal interface ITransactionLockHandler
    {
        bool TryLock(TransactionLockItem lockItem, int maxAllowedConcurrency);

        void UnLock(TransactionLockItem lockItem);
    }
}

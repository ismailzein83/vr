using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime
{
    [ServiceContract(Namespace = "http://common.vanrise.com/ITransactionLockWCFService")]
    public interface ITransactionLockWCFService
    {
        [OperationContract]
        bool TryLock(TransactionLockItem lockItem, int maxAllowedConcurrency);

        [OperationContract]
        void UnLock(TransactionLockItem lockItem);
    }
}
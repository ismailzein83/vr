using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime
{
    internal class WCFClientTransactionLockHandler : ITransactionLockHandler
    {
        string _serviceURL;
        public WCFClientTransactionLockHandler(string serviceURL)
        {
            if (String.IsNullOrWhiteSpace(serviceURL))
                throw new ArgumentNullException("serviceURL");
            _serviceURL = serviceURL;
        }

        public bool TryLock(Entities.TransactionLockItem lockItem, int maxAllowedConcurrency)
        {
            bool isLocked = false;
            Vanrise.Common.ServiceClientFactory.CreateTCPServiceClient<ITransactionLockWCFService>(_serviceURL, (client) =>
            {
                isLocked = client.TryLock(lockItem, maxAllowedConcurrency);
            });
            return isLocked;
        }

        public void UnLock(Entities.TransactionLockItem lockItem)
        {
            Vanrise.Common.ServiceClientFactory.CreateTCPServiceClient<ITransactionLockWCFService>(_serviceURL, (client) =>
            {
                client.UnLock(lockItem);
            });
        }
    }
}

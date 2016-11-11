using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Data
{
    public interface IBillingTransactionDataManager : IDataManager
    {
        IEnumerable<BillingTransaction> GetFilteredBillingTransactions(BillingTransactionQuery query);
        bool Insert(BillingTransaction billingTransaction, out long billingTransactionId);
        void GetBillingTransactionsByBalanceUpdated(Guid accountTypeId, Action<BillingTransaction> onBillingTransactionReady);

    }
}

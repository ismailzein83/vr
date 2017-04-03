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
        IEnumerable<BillingTransaction> GetBillingTransactionsForSynchronizerProcess(List<Guid> billingTransactionIds, Guid accountTypeId);
        IEnumerable<BillingTransactionMetaData> GetBillingTransactionsByAccountIds(Guid accountTypeId, List<Guid> transactionTypeIds, List<string> accountIds);
        IEnumerable<BillingTransactionMetaData> GetBillingTransactionsByTransactionTypes(Guid accountTypeId, List<BillingTransactionByTime> billingTransactionsByTime, List<Guid> transactionTypeIds);
        IEnumerable<BillingTransaction> GetBillingTransactionsByAccountId(Guid accountTypeId, String accountId);
    }
}

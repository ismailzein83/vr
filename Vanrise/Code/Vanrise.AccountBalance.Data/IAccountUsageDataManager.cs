using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Data
{
    public interface IAccountUsageDataManager : IDataManager
    {
        IEnumerable<AccountUsageInfo> GetAccountsUsageInfoByPeriod(Guid accountTypeId, DateTime periodStart, Guid transactionTypeId);
        AccountUsageInfo TryAddAccountUsageAndGet(Guid accountTypeId, Guid transactionTypeId, String accountId, DateTime periodStart, DateTime periodEnd, int currencyId, decimal usageBalance);
        IEnumerable<AccountUsage> GetAccountUsageForSpecificPeriodByAccountIds(Guid accountTypeId, Guid transactionTypeId, DateTime datePeriod, List<String> accountIds);
        List<AccountUsage> GetAccountUsageErrorData(Guid accountTypeId, Guid transactionTypeId, Guid correctionProcessId, DateTime periodDate);
        IEnumerable<AccountUsage> GetAccountUsageForBillingTransactions(Guid accountTypeId, List<Guid> transactionTypeIds, List<String> accountIds, DateTime fromTime, DateTime? toTime);
        IEnumerable<AccountUsage> GetAccountUsagesByAccount(Guid accountTypeId, String accountId);
        IEnumerable<AccountUsage> GetAccountUsagesByTransactionAccountUsageQueries(IEnumerable<TransactionAccountUsageQuery> transactionAccountUsageQueries);
        IEnumerable<AccountUsage> GetOverridenAccountUsagesByDeletedTransactionIds(IEnumerable<long> deletedTransactionIds);
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Entities;

namespace Vanrise.AccountBalance.Data
{
    public interface IAccountUsageDataManager : IDataManager
    {
        IEnumerable<AccountUsageInfo> GetAccountsUsageInfoByPeriod(Guid accountTypeId, DateTime periodStart, Guid transactionTypeId);
        AccountUsageInfo TryAddAccountUsageAndGet(Guid accountTypeId, Guid transactionTypeId, String accountId, DateTime periodStart, DateTime periodEnd, int currencyId, decimal usageBalance);
        IEnumerable<AccountUsage> GetAccountUsageForSpecificPeriodByAccountIds(Guid accountTypeId, Guid transactionTypeId, DateTime datePeriod, List<String> accountIds);
        List<AccountUsage> GetAccountUsageErrorData(Guid accountTypeId, Guid transactionTypeId, Guid correctionProcessId, DateTime periodDate);
        IEnumerable<AccountUsage> GetAccountUsageForBillingTransactions(Guid accountTypeId, List<Guid> transactionTypeIds, List<String> accountIds, DateTime fromTime, DateTime? toTime, VRAccountStatus? status, DateTime? effectiveDate, bool? isEffectiveInFuture);
        IEnumerable<AccountUsage> GetAccountUsagesByAccount(Guid accountTypeId, String accountId,VRAccountStatus? status, DateTime? effectiveDate, bool? isEffectiveInFuture);
        IEnumerable<AccountUsage> GetAccountUsagesByTransactionAccountUsageQueries(IEnumerable<TransactionAccountUsageQuery> transactionAccountUsageQueries);
        IEnumerable<AccountUsage> GetOverridenAccountUsagesByDeletedTransactionIds(IEnumerable<long> deletedTransactionIds);
        AccountUsage GetLastAccountUsage(Guid accountTypeId, string accountId);
        IEnumerable<AccountUsage> GetAccountUsagesByAccountIds(Guid accountTypeId, List<Guid> transactionTypeIds, List<string> accountIds);
        IEnumerable<AccountUsage> GetAccountUsagesByTransactionTypes(Guid accountTypeId, List<AccountUsageByTime> accountUsagesByTime, List<Guid> transactionTypeIds);
    }
}

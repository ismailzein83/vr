using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Data
{
    public interface IAccountUsageDataManager:IDataManager
    {
        IEnumerable<AccountUsage> GetPendingAccountUsages(Guid accountTypeId, long accountId);
        IEnumerable<AccountUsageInfo> GetAccountsUsageInfoByPeriod(Guid accountTypeId, DateTime periodStart, Guid transactionTypeId);
        AccountUsageInfo TryAddAccountUsageAndGet(Guid accountTypeId, Guid transactionTypeId,long accountId, DateTime periodStart, DateTime periodEnd, int currencyId, decimal usageBalance, string billingTransactionNote);
        IEnumerable<AccountUsage> GetAccountUsageForSpecificPeriodByAccountIds(Guid accountTypeId, Guid transactionTypeId, DateTime datePeriod, List<long> accountIds);
        List<AccountUsage> GetAccountUsageErrorData(Guid accountTypeId, Guid transactionTypeId, Guid correctionProcessId, DateTime periodDate);
    }
}

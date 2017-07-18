﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class AccountUsageManager
    {
        public IEnumerable<AccountUsagePeriodSettingsConfig> GetAccountUsagePeriodSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<AccountUsagePeriodSettingsConfig>(AccountUsagePeriodSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<AccountUsage> GetAccountUsageForSpecificPeriodByAccountIds(Guid accountTypeId, Guid transactionTypeId, DateTime datePeriod, List<String> accountIds)
        {
            IAccountUsageDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IAccountUsageDataManager>();
            return dataManager.GetAccountUsageForSpecificPeriodByAccountIds(accountTypeId, transactionTypeId, datePeriod, accountIds);
        }
        public List<AccountUsage> GetAccountUsageErrorData(Guid accountTypeId, Guid transactionTypeId, Guid correctionProcessId, DateTime periodDate)
        {
            IAccountUsageDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IAccountUsageDataManager>();
            return dataManager.GetAccountUsageErrorData(accountTypeId, transactionTypeId, correctionProcessId, periodDate);
        }
        public IEnumerable<AccountUsage> GetAccountUsageForBillingTransactions(Guid accountTypeId, List<Guid> transactionTypeIds, List<String> accountIds, DateTime fromTime, DateTime? toTime)
        {
            IAccountUsageDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IAccountUsageDataManager>();
            return dataManager.GetAccountUsageForBillingTransactions(accountTypeId, transactionTypeIds, accountIds, fromTime, toTime);
        }
        public IEnumerable<AccountUsage> GetAccountUsagesByAccount(Guid accountTypeId, String accountId,VRAccountStatus? status, DateTime? effectiveDate, bool? isEffectiveInFuture)
        {
            IAccountUsageDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IAccountUsageDataManager>();
            return dataManager.GetAccountUsagesByAccount(accountTypeId, accountId, status, effectiveDate, isEffectiveInFuture);
        }
        public IEnumerable<AccountUsage> GetAccountUsagesByTransactionAccountUsageQueries(IEnumerable<TransactionAccountUsageQuery> transactionAccountUsageQueries)
        {
            IAccountUsageDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IAccountUsageDataManager>();
            return dataManager.GetAccountUsagesByTransactionAccountUsageQueries(transactionAccountUsageQueries);
        }
        public IEnumerable<AccountUsage> GetOverridenAccountUsagesByDeletedTransactionIds(IEnumerable<long> deletedTransactionIds)
        {
            IAccountUsageDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IAccountUsageDataManager>();
            return dataManager.GetOverridenAccountUsagesByDeletedTransactionIds(deletedTransactionIds);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common.Business;

namespace Vanrise.AccountBalance.Business
{
    public class AccountUsageManager
    {
        public IEnumerable<AccountUsagePeriodSettingsConfig> GetAccountUsagePeriodSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<AccountUsagePeriodSettingsConfig>(AccountUsagePeriodSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<AccountUsage> GetAccountUsageForSpecificPeriodByAccountIds(Guid accountTypeId, Guid transactionTypeId, DateTime datePeriod, List<long> accountIds)
        {
            IAccountUsageDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IAccountUsageDataManager>();
            return dataManager.GetAccountUsageForSpecificPeriodByAccountIds(accountTypeId, transactionTypeId, datePeriod, accountIds);
        }
        public List<AccountUsage> GetAccountUsageErrorData(Guid accountTypeId, Guid transactionTypeId, Guid correctionProcessId, DateTime periodDate)
        {
            IAccountUsageDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IAccountUsageDataManager>();
            return dataManager.GetAccountUsageErrorData(accountTypeId, transactionTypeId, correctionProcessId, periodDate);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class AccountTypeManager
    {
        Vanrise.Common.Business.VRComponentTypeManager _vrComponentTypeManager = new Common.Business.VRComponentTypeManager();


        public string GetAccountSelector(Guid accountTypeId)
        {
            var accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            if (accountTypeSettings.ExtendedSettings == null)
                throw new NullReferenceException("accountTypeSettings.ExtendedSettings");
            return accountTypeSettings.ExtendedSettings.AccountSelector;
        }

        
        public IEnumerable<AccountTypeExtendedSettingsConfig> GetAccountBalanceExtendedSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<AccountTypeExtendedSettingsConfig>(AccountTypeExtendedSettingsConfig.EXTENSION_TYPE);
        }
        public IAccountManager GetAccountManager(Guid accountTypeId)
        {
            var accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            if (accountTypeSettings.ExtendedSettings == null)
                throw new NullReferenceException("accountTypeSettings.ExtendedSettings");
            return accountTypeSettings.ExtendedSettings.GetAccountManager();
        }



        public IEnumerable<BalanceAccountTypeInfo> GetAccountTypeInfo(AccountTypeInfoFilter filter)
        {
            Func<AccountType, bool> filterExpression = null;
            if (filter != null)
            {
                filterExpression = (item) => CheckIfFilterIsMatch(item, filter.Filters);
            }

            return _vrComponentTypeManager.GetComponentTypes<AccountTypeSettings, AccountType>().MapRecords(AccountTypeInfoMapper, filterExpression);
        }



        public bool CheckIfFilterIsMatch(AccountType accountType, List<IAccountTypeInfoFilter> filters)
        {
            AccountTypeInfoFilterContext context = new AccountTypeInfoFilterContext { AccountType = accountType };
            foreach (var filter in filters)
            {
                if (!filter.IsMatched(context))
                    return false;
            }
            return true;
        }
        public Guid GetUsageTransactionTypeId(Guid accountTypeId)
        {
            var accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            return accountTypeSettings.UsageTransactionTypeId;
        }
        public BalancePeriodSettings GetBalancePeriodSettings(Guid accountTypeId)
        {
            var accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            return accountTypeSettings.BalancePeriodSettings;
        }
        public AccountUsagePeriodSettings GetAccountUsagePeriodSettings(Guid accountTypeId)
        {
            var accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            return accountTypeSettings.AccountUsagePeriodSettings;
        }
        public AccountTypeSettings GetAccountTypeSettings(Guid accountTypeId)
        {
            return _vrComponentTypeManager.GetComponentTypeSettings<AccountTypeSettings>(accountTypeId);
        }
        private BalanceAccountTypeInfo AccountTypeInfoMapper(AccountType accountType)
        {
            string editor = null;
            if (accountType != null && accountType.Settings != null && accountType.Settings.ExtendedSettings != null)
                editor = accountType.Settings.ExtendedSettings.AccountSelector;
            return new BalanceAccountTypeInfo
            {
                Id = accountType.VRComponentTypeId,
                Name = accountType.Name,
                Editor = editor
            };
        }

      
    }
}

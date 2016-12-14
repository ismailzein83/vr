using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.AccountBalance
{
    public class AccountTypeManager
    {
        Vanrise.Common.Business.VRComponentTypeManager _vrComponentTypeManager = new Common.Business.VRComponentTypeManager();

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
        public IEnumerable<BalanceAccountTypeInfo> GetAccountTypeInfo()
        {
            return _vrComponentTypeManager.GetComponentTypes<AccountTypeSettings>().MapRecords(AccountTypeInfoMapper);
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
        private BalanceAccountTypeInfo AccountTypeInfoMapper(VRComponentType<AccountTypeSettings> componentType)
        {
            return new BalanceAccountTypeInfo
            {
                Id = componentType.VRComponentTypeId,
                Name = componentType.Name
            };
        }

        AccountTypeSettings GetAccountTypeSettings(Guid accountTypeId)
        {
            return _vrComponentTypeManager.GetComponentTypeSettings<AccountTypeSettings>(accountTypeId);
        }
        BalanceAccountTypeInfo AccountTypeInfoMapper(AccountType accountType)
        {
            return new BalanceAccountTypeInfo
            {
                Id = accountType.VRComponentTypeId,
                Name = accountType.Name
            };
        }
    }
}

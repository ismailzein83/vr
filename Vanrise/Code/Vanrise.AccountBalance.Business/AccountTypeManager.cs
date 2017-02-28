using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;

namespace Vanrise.AccountBalance.Business
{
    public class AccountTypeManager : IAccountTypeManager
    {
        Vanrise.Common.Business.VRComponentTypeManager _vrComponentTypeManager = new Common.Business.VRComponentTypeManager();
        static SecurityManager s_securityManager = new SecurityManager();


        public string GetAccountSelector(Guid accountTypeId)
        {
            var accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            if (accountTypeSettings.ExtendedSettings == null)
                throw new NullReferenceException("accountTypeSettings.ExtendedSettings");
            return accountTypeSettings.ExtendedSettings.AccountSelector;
        }
        public string GetAccountTypeName(Guid accountTypeId)
        {
            var accountType = _vrComponentTypeManager.GetComponentType<AccountTypeSettings,AccountType>(accountTypeId);
            accountType.ThrowIfNull("accountType", accountTypeId);
            return accountType.Name;
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
        public TimeSpan GetTimeOffset(Guid accountTypeId)
        {
            var accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            return accountTypeSettings.TimeOffset;
        }
        public AccountTypeSettings GetAccountTypeSettings(Guid accountTypeId)
        {
            return _vrComponentTypeManager.GetComponentTypeSettings<AccountTypeSettings>(accountTypeId);
        }

        public bool DoesUserHaveViewAccess(int userId, List<Guid> AccountTypeIds)
        {
            foreach (var a in AccountTypeIds)
            {
                if (DoesUserHaveViewAccess(userId, a))
                    return true;
            }
            return false;
        }

        public bool DoesUserHaveViewAccess(Guid accountTypeId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            var accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            return DoesUserHaveViewAccess(userId, accountTypeSettings);
        }
        public bool DoesUserHaveViewAccess(int userId, Guid accountTypeId)
        {
            var accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            return DoesUserHaveViewAccess(userId, accountTypeSettings);
        }
        public bool DoesUserHaveViewAccess(int userId, AccountTypeSettings accountTypeSettings)
        {
            if (accountTypeSettings.Security != null && accountTypeSettings.Security.ViewRequiredPermission != null)
                return s_securityManager.IsAllowed(accountTypeSettings.Security.ViewRequiredPermission, userId);
            else
                return true;
        }

        public bool DoesUserHaveAddAccess(Guid accountBeDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveAddAccess(userId, accountBeDefinitionId);
        }
        public bool DoesUserHaveAddAccess(int userId, Guid accountBeDefinitionId)
        {
            var accountBEDefinitionSettings = GetAccountTypeSettings(accountBeDefinitionId);
            if (accountBEDefinitionSettings != null && accountBEDefinitionSettings.Security != null && accountBEDefinitionSettings.Security.AddRequiredPermission != null)
                return s_securityManager.IsAllowed(accountBEDefinitionSettings.Security.AddRequiredPermission, userId);
            else
                return true;
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

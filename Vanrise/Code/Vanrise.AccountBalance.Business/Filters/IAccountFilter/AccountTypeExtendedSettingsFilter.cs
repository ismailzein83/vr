using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class AccountTypeExtendedSettingsFilter<T> : IAccountTypeExtendedSettingsFilter where T : AccountTypeExtendedSettings
    {
        public bool IsMatched(IAccountTypeExtendedSettingsFilterContext context)
        {
            if (context.AccountType.Settings == null || context.AccountType.Settings.ExtendedSettings == null)
                return false;

            var accountBalanceSetting = context.AccountType.Settings.ExtendedSettings as T;
            if (accountBalanceSetting == null)
                return false;

            return true;
        }
    }
}

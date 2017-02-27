using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Entities;
using Vanrise.Common;

namespace TOne.WhS.AccountBalance.Business
{
    public class FinancialAccountDefinitionManager
    {
        static Vanrise.AccountBalance.Business.AccountTypeManager s_accountTypeManager = new Vanrise.AccountBalance.Business.AccountTypeManager();
        public T GetFinancialAccountDefinitionExtendedSettings<T>(Guid accountTypeId) where T : AccountBalanceSettings
        {
            var accountTypeSettings = s_accountTypeManager.GetAccountTypeSettings(accountTypeId);
            accountTypeSettings.ThrowIfNull("accountType", accountTypeId);
            return accountTypeSettings.ExtendedSettings.CastWithValidate<T>("accountTypeSettings.ExtendedSettings", accountTypeId);
        }
    }
}

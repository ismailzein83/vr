using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace TOne.WhS.AccountBalance.Business
{
    public class AccountBalanceSettingsFilter : IAccountTypeInfoFilter
    {
        public long? CarrierProfileId { get; set; }
        public long? CarrierAccountId { get; set; }

        public bool IsMatched(IAccountTypeInfoFilterContext context)
        {
            var accountBalanceSetting = context.AccountType.Settings.ExtendedSettings as AccountBalanceSettings;
            if (accountBalanceSetting == null)
                return false;
            return true;
        }
    }
}

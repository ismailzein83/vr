using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.AccountBalance.Business;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;


namespace Vanrise.AccountBalance.Business
{
    public class AccountTypeViewFilter : IAccountTypeInfoFilter
    {

        public Guid ViewId { get; set; }

        public bool IsMatched(IAccountTypeInfoFilterContext context)
        {
            ViewManager viewManager = new ViewManager();
            var accountBalancesViewSettings = viewManager.GetView(this.ViewId).Settings as IAccountBalanceViewSettings;

            if (!accountBalancesViewSettings.GetAccountTypeIds().Contains(context.AccountType.VRComponentTypeId))
                return false;

            return true;
        }

    }
}

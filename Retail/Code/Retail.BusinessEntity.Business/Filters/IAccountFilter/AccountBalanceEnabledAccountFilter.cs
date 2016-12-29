using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountBalanceEnabledAccountFilter : IAccountFilter
    {
        public bool IsExcluded(IAccountFilterContext context)
        {
            if (context.Account != null)
            {
                AccountManager accountManager = new AccountManager();
                IAccountPayment accountPayment;
                return !accountManager.HasAccountPayment(context.Account.AccountId, false, out accountPayment);
            }
            return false;
        }
    }
}

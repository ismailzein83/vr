using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Invoice.Business
{
    public class InvoiceEnabledAccountFilter : IAccountFilter
    {
        public bool IsExcluded(IAccountFilterContext context)
        {
            if (context.Account != null)
            {
                AccountManager accountManager = new AccountManager();
                IAccountPayment accountPayment;
                return !(accountManager.HasAccountPayment(context.Account.AccountId, true, out accountPayment));
            }
            return false;
        }
    }
}

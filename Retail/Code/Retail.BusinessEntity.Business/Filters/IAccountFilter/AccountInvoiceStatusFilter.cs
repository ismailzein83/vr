using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business.Filters
{
    class AccountInvoiceStatusFilter : IAccountFilter
    {
        public VRAccountStatus? Status { get; set; }

        public bool IsExcluded(IAccountFilterContext context)
        {
            if (!Status.HasValue)
                return false;
            AccountBEManager accountBEManager = new AccountBEManager();
            switch (this.Status.Value)
            {
                case VRAccountStatus.Active:
                    return !accountBEManager.IsAccountInvoiceActive(context.Account);
                case VRAccountStatus.InActive:
                    return accountBEManager.IsAccountInvoiceActive(context.Account);
            }
            return false;
        }
    }
}

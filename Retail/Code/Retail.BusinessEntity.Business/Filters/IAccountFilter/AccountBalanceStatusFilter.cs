using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business.Filters
{
    public class AccountBalanceStatusFilter:IAccountFilter
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
                    return !accountBEManager.IsAccountBalanceActive(context.Account);
                case VRAccountStatus.InActive:
                    return accountBEManager.IsAccountBalanceActive(context.Account);
            }
            return false;
        }
    }
}

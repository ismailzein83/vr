using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountConditionAccountFilter : IAccountFilter
    {
        public AccountCondition AccountCondition { get; set; }

        public bool IsExcluded(IAccountFilterContext context)
        {
            if (context.Account != null && this.AccountCondition != null)
            {
                return !new AccountBEManager().EvaluateAccountCondition(context.Account, this.AccountCondition);
            }
            return false;
        }
    }
}

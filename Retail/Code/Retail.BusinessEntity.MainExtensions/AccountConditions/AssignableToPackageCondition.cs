using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountConditions
{
    public class AssignableToPackageCondition : AccountCondition
    {
        public override Guid ConfigId
        {
            get { return new Guid("20ABA92B-ECB3-497E-B136-59E4C71BD3B7"); }
        }

        public override bool Evaluate(IAccountConditionEvaluationContext context)
        {
            return new AccountBEManager().IsAccountAssignableToPackage(context.Account);
        }
    }
}

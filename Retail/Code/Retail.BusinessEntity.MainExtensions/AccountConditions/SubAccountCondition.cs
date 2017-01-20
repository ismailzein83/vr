using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountConditions
{
    public class SubAccountCondition : AccountCondition
    {
        public override Guid ConfigId { get { return new Guid("385AB73F-D18D-4A1B-8552-FC4E6AC487DE"); } }

        public override bool Evaluate(IAccountConditionEvaluationContext context)
        {
            return new AccountTypeManager().CanHaveSubAccounts(context.Account);
        }
    }
}

using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountConditions
{
    public class FinancialAccountCondition : AccountCondition
    {
        public override Guid ConfigId { get { return new Guid("EE17B999-5473-467F-A9BF-623EEF6CD409"); } }

        public override bool Evaluate(IAccountConditionEvaluationContext context)
        {
            return new AccountBEManager().IsFinancial(context.Account);
        }
    }
}

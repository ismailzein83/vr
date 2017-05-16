using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Business
{
    public class AccountBERuntimeSelectorFilter //: BERuntimeSelectorFilter
    {
        //public AccountCondition AccountCondition { get; set; }

        //public override bool IsMatched(IBERuntimeSelectorFilterSelectorFilterContext context)
        //{
        //    if (AccountCondition == null)
        //        return true;

        //    long accountId;
        //    if (!long.TryParse(context.BusinessEntityId.ToString(), out accountId))
        //        return false;

        //    var accountConditionEvaluationContext = new AccountConditionEvaluationContext(context.BusinessEntityDefinitionId, accountId);
        //    return AccountCondition.Evaluate(accountConditionEvaluationContext);
        //}
    }
}

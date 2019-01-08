using System;
using Vanrise.Data.RDB;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class BEDataUtility
    {
        public static void SetEffectiveAfterDateCondition(RDBConditionContext conditionContext, string colBED, string ColEED, DateTime effectiveOn)
        {
            conditionContext.LessOrEqualCondition(colBED).Value(effectiveOn);
            var orCondition = conditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(ColEED);
            orCondition.GreaterThanCondition(ColEED).Value(effectiveOn);
        }
        private void AddEffectiveAfterDateCondition(RDBConditionContext context, string colBED, string colEED, DateTime effectiveAfter)
        {
            var effectiveCondition = context.ChildConditionGroup(RDBConditionGroupOperator.OR);
            effectiveCondition.NullCondition(colEED);
            effectiveCondition.GreaterThanCondition(colBED).Value(effectiveAfter);
        }
    }
}

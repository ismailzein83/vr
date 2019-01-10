using System;
using Vanrise.Data.RDB;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class BEDataUtility
    {
        public static void SetEffectiveAfterDateCondition(RDBConditionContext conditionContext,string tableAlias, string colBED, string ColEED, DateTime effectiveOn)
        {
            var andConditionContext = conditionContext.ChildConditionGroup();
            andConditionContext.LessOrEqualCondition(tableAlias,colBED).Value(effectiveOn);
            var orCondition = andConditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(tableAlias, ColEED);
            orCondition.GreaterThanCondition(tableAlias, ColEED).Value(effectiveOn);
        }
        public static void SetFutureDateCondition(RDBConditionContext context, string tableAlias, string colBED, string colEED, DateTime effectiveAfter)
        {
            var effectiveCondition = context.ChildConditionGroup(RDBConditionGroupOperator.OR);
            effectiveCondition.NullCondition(tableAlias, colEED);
            effectiveCondition.GreaterThanCondition(tableAlias, colBED).Value(effectiveAfter);
        }
    }
}

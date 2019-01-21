using System;
using Vanrise.Data.RDB;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class BEDataUtility
    {
        public static void SetEffectiveDateCondition(RDBConditionContext conditionContext, string tableAlias, string colBED, string ColEED, DateTime effectiveOn)
        {
            //((sc.BED <= @when ) and (sc.EED is null or sc.EED > @when))
            var andConditionContext = conditionContext.ChildConditionGroup();
            andConditionContext.LessOrEqualCondition(tableAlias, colBED).Value(effectiveOn);
            var orCondition = andConditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(tableAlias, ColEED);
            orCondition.GreaterThanCondition(tableAlias, ColEED).Value(effectiveOn);
        }
        public static void SetFutureDateCondition(RDBConditionContext context, string tableAlias, string colBED, string colEED, DateTime effectiveAfter)
        {
            //(BED > effectiveAfter OR EED IS NULL)
            var effectiveCondition = context.ChildConditionGroup(RDBConditionGroupOperator.OR);
            effectiveCondition.NullCondition(tableAlias, colEED);
            effectiveCondition.GreaterThanCondition(tableAlias, colBED).Value(effectiveAfter);
        }

        public static void SetEffectiveAfterDateCondition(RDBConditionContext context, string tableAlias, string colBED, string ColEED, DateTime effectiveOn)
        {
            //(sc.EED is null or (sc.EED<>sc.BED and sc.EED > @when))
            var effectiveAfterDateCondition = context.ChildConditionGroup(RDBConditionGroupOperator.OR);
            effectiveAfterDateCondition.NullCondition(ColEED);

            var dateAndCondition = effectiveAfterDateCondition.ChildConditionGroup();
            dateAndCondition.NotEqualsCondition(ColEED).Column(colBED);
            dateAndCondition.GreaterThanCondition(ColEED).Value(effectiveOn);
        }

        public static void SetParentCodeCondition(RDBConditionContext conditionContext, string code, string tableAlias, string codeColumnName)
        {
            var andConditionContext = conditionContext.ChildConditionGroup();
            var compareCondition = andConditionContext.CompareCondition(RDBCompareConditionOperator.StartWith);
            compareCondition.Expression1().Value(code);
            compareCondition.Expression2().Column(tableAlias, codeColumnName);
        }

        public static void SetDateCondition(RDBConditionContext context, string tableAlias, string colBED, string ColEED, bool isFuture, DateTime? effectiveOn)
        {
            if (effectiveOn.HasValue)
            {
                if (isFuture)
                    SetEffectiveDateCondition(context, tableAlias, colBED, ColEED, effectiveOn.Value);
                else
                    SetFutureDateCondition(context, tableAlias, colBED, ColEED, effectiveOn.Value);
            }
            else
                context.FalseCondition();
        }
    }
}

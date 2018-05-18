using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.AccountBalance.Data.RDB
{
    public static class RDBExtensionMethods
    {
        public static T LiveBalanceActiveAndEffectiveCondition<T>(this RDBConditionContext<T> conditionContext, string liveBalanceAlias, VRAccountStatus? accountStatus, DateTime? effectiveDate, bool? isEffectiveInFuture)
        {
            return conditionContext.Or().NullCondition(liveBalanceAlias, "AccountID")
                                     .And()
                                            .ConditionIfNotDefaultValue(accountStatus, ctx => ctx.EqualsCondition(liveBalanceAlias, "Status", (int)accountStatus.Value))
                                            .ConditionIfNotDefaultValue(effectiveDate, ctx => ctx.And()
                                                                                                     .ConditionIfColumnNotNull(liveBalanceAlias, "BED").CompareCondition(liveBalanceAlias, "BED", RDBCompareConditionOperator.LEq, effectiveDate.Value)
                                                                                                     .ConditionIfColumnNotNull(liveBalanceAlias, "EED").CompareCondition(liveBalanceAlias, "EED", RDBCompareConditionOperator.G, effectiveDate.Value)
                                                                                                 .EndAnd()
                                                                        )
                                            .ConditionIfNotDefaultValue(isEffectiveInFuture, ctx => ctx.ConditionIf(
                                                                                                        () => isEffectiveInFuture.Value, 
                                                                                                        isEffectiveInFutureCtx => isEffectiveInFutureCtx.ConditionIfColumnNotNull(liveBalanceAlias, "EED").CompareCondition(liveBalanceAlias, "EED", RDBCompareConditionOperator.GEq, new RDBNowDateTimeExpression()),
                                                                                                        notEffectiveInFutureCtx => notEffectiveInFutureCtx.And()
                                                                                                                                                            .NotNullCondition(liveBalanceAlias, "EED")
                                                                                                                                                            .CompareCondition(liveBalanceAlias, "EED", RDBCompareConditionOperator.LEq, new RDBNowDateTimeExpression())
                                                                                                                                                            .EndAnd()
                                                                                                                    )
                                                                                                       
                                                                        )
                                     .EndAnd()
                                   .EndOr();
        }

        public static IRDBJoinContextReady<T> JoinLiveBalance<T>(this RDBJoinContext<T> joinContext, string liveBalanceAlias, string originalTableAlias)
        {
            return joinContext
                .Join(RDBJoinType.Left, LiveBalanceDataManager.TABLE_NAME, liveBalanceAlias)
                    .And()
                        .EqualsCondition(liveBalanceAlias, "AccountTypeID", originalTableAlias, "AccountTypeID")
                        .EqualsCondition(liveBalanceAlias, "AccountID", originalTableAlias, "AccountID")
                    .EndAnd();
        }
    }
}

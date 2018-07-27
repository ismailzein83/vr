using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
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
                                            .ConditionIfColumnNotNull(liveBalanceAlias, "IsDeleted").EqualsCondition(liveBalanceAlias, "IsDeleted", false)
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

        public static IRDBBatchQuery<IRDBQueryContextReady> UpdateLiveBalances(this IRDBBatchQuery<IRDBQueryContextReady> batchQueryContext, IEnumerable<LiveBalanceToUpdate> liveBalancesToUpdate)
        {
            if (liveBalancesToUpdate != null && liveBalancesToUpdate.Count() > 0)
            {
                var tempTableColumns = new Dictionary<string, RDBTableColumnDefinition> 
                {
                    {"ID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt}},
                    {"UpdateValue", new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6}}
                };
                var tempTableQuery = new RDBTempTableQuery(tempTableColumns);
                return batchQueryContext
                    .AddQuery().CreateTempTable(tempTableQuery)
                    .Foreach(liveBalancesToUpdate, (lvToUpdate, context) => context.AddQuery().Insert().IntoTable(tempTableQuery).ColumnValue("ID", lvToUpdate.LiveBalanceId).ColumnValue("UpdateValue", lvToUpdate.Value).EndInsert())
                    .AddQuery().Update().FromTable(LiveBalanceDataManager.TABLE_NAME)
                                        .Join("lv").JoinOnEqualOtherTableColumn(tempTableQuery, "lvToUpdate", "ID", "lv", "ID").EndJoin()
                                        .ColumnValue("CurrentBalance", new RDBArithmeticExpression
                                        {
                                            Operator = RDBArithmeticExpressionOperator.Add,
                                            Expression1 = new RDBColumnExpression { TableAlias = "lv", ColumnName = "CurrentBalance", DontAppendTableAlias = true },
                                            Expression2 = new RDBColumnExpression { TableAlias = "lvToUpdate", ColumnName = "UpdateValue" }
                                        })
                                        .EndUpdate();
            }
            else
            {
                return batchQueryContext;
            }
        }


        public static IRDBBatchQuery<IRDBQueryContextReady> UpdateAccountUsages(this IRDBBatchQuery<IRDBQueryContextReady> batchQueryContext, IEnumerable<AccountUsageToUpdate> accountUsagesToUpdate, Guid? correctionProcessId)
        {
            if (accountUsagesToUpdate != null && accountUsagesToUpdate.Count() > 0)
            {
                var tempTableColumns = new Dictionary<string, RDBTableColumnDefinition> 
                {
                    {"ID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt}},
                    {"UpdateValue", new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6}}
                };
                var tempTableQuery = new RDBTempTableQuery(tempTableColumns);
                return batchQueryContext
                     .AddQuery().CreateTempTable(tempTableQuery)
                     .Foreach(accountUsagesToUpdate, (auToUpdate, context) => context.AddQuery().Insert().IntoTable(tempTableQuery).ColumnValue("ID", auToUpdate.AccountUsageId).ColumnValue("UpdateValue", auToUpdate.Value).EndInsert())
                     .AddQuery().Update().FromTable(AccountUsageDataManager.TABLE_NAME)
                                         .Join("au").JoinOnEqualOtherTableColumn(tempTableQuery, "auToUpdate", "ID", "au", "ID").EndJoin()
                                         .ColumnValue("UsageBalance", new RDBArithmeticExpression
                                         {
                                             Operator = RDBArithmeticExpressionOperator.Add,
                                             Expression1 = new RDBColumnExpression { TableAlias = "au", ColumnName = "UsageBalance", DontAppendTableAlias = true },
                                             Expression2 = new RDBColumnExpression { TableAlias = "auToUpdate", ColumnName = "UpdateValue" }
                                         })
                                         .ColumnValueDBNullIfDefaultValue("CorrectionProcessID", correctionProcessId)
                                         .EndUpdate();
            }
            else
            {
                return batchQueryContext;
            }
        }

        public static RDBAndConditionContext<T> LiveBalanceToAlertCondition<T>(this RDBAndConditionContext<T> andConditionContext, string liveBalanceAlias)
        {
            return andConditionContext
                    .CompareCondition("CurrentBalance", RDBCompareConditionOperator.LEq, new RDBColumnExpression { TableAlias = liveBalanceAlias, ColumnName = "NextAlertThreshold" })
                    .ConditionIfColumnNotNull("LastExecutedActionThreshold").CompareCondition("NextAlertThreshold", RDBCompareConditionOperator.L, new RDBColumnExpression { TableAlias = liveBalanceAlias, ColumnName = "LastExecutedActionThreshold" });
        }
        public static RDBAndConditionContext<T> LiveBalanceToClearAlertCondition<T>(this RDBAndConditionContext<T> andConditionContext, string liveBalanceAlias)
        {
            return andConditionContext
                    .CompareCondition("CurrentBalance", RDBCompareConditionOperator.G, new RDBColumnExpression { TableAlias = liveBalanceAlias, ColumnName = "LastExecutedActionThreshold" });
        }

        public static RDBAndConditionContext<T> BillingTransactionsToUpdateBalanceCondition<T>(this RDBAndConditionContext<T> andConditionContext)
        {
            return andConditionContext
                                .Or()
                                    .And()
                                        .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
                                        .ConditionIfColumnNotNull("IsBalanceUpdated").EqualsCondition("IsBalanceUpdated", false)
                                    .EndAnd()
                                    .And()
                                        .NotNullCondition("IsDeleted")
                                        .EqualsCondition("IsDeleted", true)
                                        .NotNullCondition("IsBalanceUpdated")
                                        .EqualsCondition("IsBalanceUpdated", true)
                                        .ConditionIfColumnNotNull("IsSubtractedFromBalance").EqualsCondition("IsSubtractedFromBalance", false)
                                    .EndAnd()
                                .EndOr();
        }

        public static IRDBInsertQueryColumnsAssigned<T> AddInsertBillingTransactionColumns<T>(this IRDBInsertQueryCanAssignColumns<T> insertQuery, BillingTransaction billingTransaction)
        {
            string serializedSettings = Vanrise.Common.Serializer.Serialize(billingTransaction.Settings);
            return insertQuery
                    .ColumnValue("AccountID", billingTransaction.AccountId)
                    .ColumnValue("AccountTypeID", billingTransaction.AccountTypeId)
                    .ColumnValue("Amount", billingTransaction.Amount)
                    .ColumnValue("CurrencyId", billingTransaction.CurrencyId)
                    .ColumnValue("TransactionTypeID", billingTransaction.TransactionTypeId)
                    .ColumnValue("TransactionTime", billingTransaction.TransactionTime)
                    .ColumnValue("Notes", billingTransaction.Notes)
                    .ColumnValue("Reference", billingTransaction.Reference)
                    .ColumnValue("SourceID", billingTransaction.SourceId)
                    .ColumnValueIf(() => billingTransaction.Settings != null, ctx => ctx.ColumnValue("Settings", serializedSettings));
        }

        public static IRDBBatchQueryQueryAdded<T> InsertInvoiceBillingTransactions<T>(this IRDBBatchQuery<T> batchQueryContext, IEnumerable<BillingTransaction> billingTransactions, long invoiceId)
        {
            return batchQueryContext.Foreach(billingTransactions,
                                                    (billingTransaction, ctx) =>
                                                    ctx
                                                    .AddQuery()
                                                    .Insert()
                                                    .IntoTable(BillingTransactionDataManager.TABLE_NAME)
                                                    .AddInsertBillingTransactionColumns(billingTransaction)
                                                    .ColumnValue("CreatedByInvoiceID", invoiceId)
                                                    .EndInsert());
        }

        public static IRDBBatchQueryQueryAdded<T> SetInvoiceBillingTransactionsDeleted<T>(this IRDBBatchQuery<T> batchQueryContext, long invoiceId)
        {
            return batchQueryContext
                    .AddQuery()
                    .Update()
                    .FromTable(BillingTransactionDataManager.TABLE_NAME)
                    .Where().And().NotNullCondition("CreatedByInvoiceID").EqualsCondition("CreatedByInvoiceID", invoiceId).EndAnd()
                    .ColumnValue("IsDeleted", true)
                    .EndUpdate();
        }

    }
}
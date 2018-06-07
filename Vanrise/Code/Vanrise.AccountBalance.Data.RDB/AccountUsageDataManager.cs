using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Data.RDB;

namespace Vanrise.AccountBalance.Data.RDB
{
    public class AccountUsageDataManager : IAccountUsageDataManager
    {
        public static string TABLE_NAME = "VR_AccountBalance_AccountUsage";

        static AccountUsageDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add("ID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("AccountTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("AccountID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add("TransactionTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("CurrencyId", new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add("UsageBalance", new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6 });
            columns.Add("PeriodStart", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("PeriodEnd", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("IsOverridden", new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add("OverriddenAmount", new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6 });
            columns.Add("CorrectionProcessID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("CreatedTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_AccountBalance",
                DBTableName = "AccountUsage",
                Columns = columns,
                IdColumnName = "ID",
                CreatedTimeColumnName = "CreatedTime"
            });
        }

        #region Private Methods

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_AccountBalance", "VR_AccountBalance_TransactionDBConnStringKey", "VR_AccountBalance_TransactionDBConnString");
        }

        private AccountUsage AccountUsageMapper(IRDBDataReader reader)
        {
            return new AccountUsage
            {
                AccountUsageId = reader.GetLong("ID"),
                AccountId = reader.GetString("AccountID"),
                TransactionTypeId = reader.GetGuid("TransactionTypeID"),
                AccountTypeId = reader.GetGuid("AccountTypeId"),
                PeriodStart = reader.GetDateTime("PeriodStart"),
                PeriodEnd = reader.GetDateTime("PeriodEnd"),
                UsageBalance = reader.GetDecimal("UsageBalance"),
                CurrencyId = reader.GetInt("CurrencyId"),
                IsOverriden = reader.GetBooleanWithNullHandling("IsOverridden"),
                OverridenAmount = reader.GetNullableDecimal("OverriddenAmount"),
                CorrectionProcessId = reader.GetNullableGuid("CorrectionProcessID")
            };
        }
        private AccountUsageInfo AccountUsageInfoMapper(IRDBDataReader reader)
        {
            return new AccountUsageInfo
            {
                AccountUsageId = reader.GetLong("ID"),
                AccountId = reader.GetString("AccountID"),
                TransactionTypeId = reader.GetGuid("TransactionTypeID"),
                IsOverridden = reader.GetBooleanWithNullHandling("IsOverridden"),
            };
        }

        #endregion

        #region IAccountUsageDataManager

        public IEnumerable<AccountUsageInfo> GetAccountsUsageInfoByPeriod(Guid accountTypeId, DateTime periodStart, Guid transactionTypeId)
        {
            return new RDBQueryContext(GetDataProvider())
                        .Select()
                        .From(TABLE_NAME, "au")
                        .Where().And()
                                    .EqualsCondition("AccountTypeID", accountTypeId)
                                    .EqualsCondition("PeriodStart", periodStart)
                                    .EqualsCondition("TransactionTypeID", transactionTypeId)
                                .EndAnd()
                        .SelectColumns().Columns("ID", "AccountID", "TransactionTypeID", "IsOverridden").EndColumns()
                        .EndSelect()
                        .GetItems(AccountUsageInfoMapper);
        }

        public AccountUsageInfo TryAddAccountUsageAndGet(Guid accountTypeId, Guid transactionTypeId, string accountId, DateTime periodStart, DateTime periodEnd, int currencyId, decimal usageBalance)
        {
            var accountUsageInfo = new AccountUsageInfo
            {
                AccountId = accountId,
                TransactionTypeId = transactionTypeId
            };
            bool accountUsageFound = false;
            new RDBQueryContext(GetDataProvider())
                    .Select()
                    .From(TABLE_NAME, "au")
                    .Where().And()
                                .EqualsCondition("AccountTypeID", accountTypeId)
                                .EqualsCondition("AccountID", accountId)
                                .EqualsCondition("TransactionTypeID", transactionTypeId)
                                .EqualsCondition("PeriodStart", periodStart)
                            .EndAnd()
                    .SelectColumns().Columns("ID", "IsOverridden").EndColumns()
                    .EndSelect()
                    .ExecuteReader(reader =>
                    {
                        if(reader.Read())
                        {
                            accountUsageFound = true;
                            accountUsageInfo.AccountUsageId = reader.GetLong("ID");
                            accountUsageInfo.IsOverridden = reader.GetBooleanWithNullHandling("IsOverridden");
                        }
                    });
            if(!accountUsageFound)
            {
                accountUsageInfo.IsOverridden = new RDBQueryContext(GetDataProvider())
                    .Select()
                    .From(AccountUsageOverrideDataManager.TABLE_NAME, "au_override", 1)
                    .Where().And()
                                .EqualsCondition("AccountTypeID", accountTypeId)
                                .EqualsCondition("AccountID", accountId)
                                .EqualsCondition("TransactionTypeID", transactionTypeId)
                                .CompareCondition("PeriodStart", RDBCompareConditionOperator.LEq, periodStart)
                                .CompareCondition("PeriodEnd", RDBCompareConditionOperator.GEq, periodEnd)
                            .EndAnd()
                    .SelectColumns().Column("ID").EndColumns()
                    .EndSelect()
                    .ExecuteScalar().NullableLongValue.HasValue;

                accountUsageInfo.AccountUsageId = new RDBQueryContext(GetDataProvider())
                    .Insert()
                    .IntoTable(TABLE_NAME)
                    .GenerateIdAndAssignToParameter("AccountUsageId")
                    .ColumnValue("AccountTypeID", accountTypeId)
                    .ColumnValue("AccountID", accountId)
                    .ColumnValue("TransactionTypeID", transactionTypeId)
                    .ColumnValue("CurrencyId", currencyId)
                    .ColumnValue("PeriodStart", periodStart)
                    .ColumnValue("PeriodEnd", periodEnd)
                    .ColumnValue("UsageBalance", usageBalance)
                    .ColumnValue("IsOverridden", accountUsageInfo.IsOverridden)
                    .ColumnValueIf(() => accountUsageInfo.IsOverridden, ctx => ctx.ColumnValue("OverriddenAmount", 0))
                    .EndInsert()
                    .ExecuteScalar().LongValue;
            }
            return accountUsageInfo;
        }

        public IEnumerable<AccountUsage> GetAccountUsageForSpecificPeriodByAccountIds(Guid accountTypeId, Guid transactionTypeId, DateTime datePeriod, List<string> accountIds)
        {
            return new RDBQueryContext(GetDataProvider())
                        .Select()
                        .From(TABLE_NAME, "au")
                        .Where().And()
                                    .EqualsCondition("AccountTypeID", accountTypeId)
                                    .ConditionIf(() => accountIds != null && accountIds.Count > 0, ctx => ctx.ListCondition("AccountID", RDBListConditionOperator.IN, accountIds))
                                    .EqualsCondition("PeriodStart", datePeriod)
                                    .EqualsCondition("TransactionTypeID", transactionTypeId)
                                .EndAnd()
                        .SelectColumns().AllTableColumns("au").EndColumns()
                        .EndSelect()
                        .GetItems(AccountUsageMapper);
        }

        public List<AccountUsage> GetAccountUsageErrorData(Guid accountTypeId, Guid transactionTypeId, Guid correctionProcessId, DateTime periodDate)
        {
            return new RDBQueryContext(GetDataProvider())
                         .Select()
                         .From(TABLE_NAME, "au")
                         .Where().And()
                                     .EqualsCondition("AccountTypeID", accountTypeId)
                                     .EqualsCondition("PeriodStart", periodDate)
                                     .EqualsCondition("TransactionTypeID", transactionTypeId)
                                     .ConditionIfColumnNotNull("CorrectionProcessID").CompareCondition("CorrectionProcessID", RDBCompareConditionOperator.NEq, correctionProcessId)
                                 .EndAnd()
                         .SelectColumns().AllTableColumns("au").EndColumns()
                         .EndSelect()
                         .GetItems(AccountUsageMapper);
        }

        public IEnumerable<AccountUsage> GetAccountUsageForBillingTransactions(Guid accountTypeId, List<Guid> transactionTypeIds, List<string> accountIds, DateTime fromTime, DateTime? toTime, Vanrise.Entities.VRAccountStatus? status, DateTime? effectiveDate, bool? isEffectiveInFuture)
        {
            return new RDBQueryContext(GetDataProvider())
                         .Select()
                         .From(TABLE_NAME, "au")
                         .Join()
                         .JoinLiveBalance("lb", "au")
                         .EndJoin()
                         .Where().And()
                                     .EqualsCondition("AccountTypeID", accountTypeId)
                                     .ConditionIf(() => accountIds != null && accountIds.Count > 0, ctx => ctx.ListCondition("AccountID", RDBListConditionOperator.IN, accountIds))
                                     .ConditionIf(() => transactionTypeIds != null && transactionTypeIds.Count > 0, ctx => ctx.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, transactionTypeIds))
                                     .CompareCondition("PeriodStart", RDBCompareConditionOperator.GEq, fromTime)
                                     .ConditionIfNotDefaultValue(toTime, ctx => ctx.CompareCondition("PeriodStart", RDBCompareConditionOperator.LEq, toTime.Value))
                                     .ConditionIfColumnNotNull("IsOverridden").EqualsCondition("IsOverridden", false)
                                     .LiveBalanceActiveAndEffectiveCondition("lb", status, effectiveDate, isEffectiveInFuture)
                                 .EndAnd()
                         .SelectColumns().AllTableColumns("au").EndColumns()
                         .EndSelect()
                         .GetItems(AccountUsageMapper);
        }

        public IEnumerable<AccountUsage> GetAccountUsagesByAccount(Guid accountTypeId, string accountId, Vanrise.Entities.VRAccountStatus? status, DateTime? effectiveDate, bool? isEffectiveInFuture)
        {
            return new RDBQueryContext(GetDataProvider())
                         .Select()
                         .From(TABLE_NAME, "au")
                         .Join()
                         .JoinLiveBalance("lb", "au")
                         .EndJoin()
                         .Where().And()
                                     .EqualsCondition("AccountTypeID", accountTypeId)
                                     .EqualsCondition("AccountID", accountId)
                                     .ConditionIfColumnNotNull("IsOverridden").EqualsCondition("IsOverridden", false)
                                     .LiveBalanceActiveAndEffectiveCondition("lb", status, effectiveDate, isEffectiveInFuture)
                                 .EndAnd()
                         .SelectColumns().AllTableColumns("au").EndColumns()
                         .EndSelect()
                         .GetItems(AccountUsageMapper);
        }

        public IEnumerable<AccountUsage> GetAccountUsagesByTransactionAccountUsageQueries(IEnumerable<TransactionAccountUsageQuery> transactionAccountUsageQueries)
        {
            var tempTableColumns = new Dictionary<string, RDBTableColumnDefinition>();
            tempTableColumns.Add("TransactionID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            tempTableColumns.Add("TransactionTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            tempTableColumns.Add("AccountTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            tempTableColumns.Add("AccountID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            tempTableColumns.Add("PeriodStart", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            tempTableColumns.Add("PeriodEnd", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            var tempTableQuery = new RDBTempTableQuery(tempTableColumns);
            return new RDBQueryContext(GetDataProvider())
                        .StartBatchQuery()
                            .AddQuery().CreateTempTable(tempTableQuery)
                            .Foreach(transactionAccountUsageQueries, (queryItem, ctx) =>
                                        ctx.AddQuery()
                                            .Insert()
                                            .IntoTable(tempTableQuery)
                                            .ColumnValue("TransactionID", queryItem.TransactionId)
                                            .ColumnValue("TransactionTypeID", queryItem.TransactionTypeId)
                                            .ColumnValue("AccountTypeID", queryItem.AccountTypeId)
                                            .ColumnValue("AccountID", queryItem.AccountId)
                                            .ColumnValue("PeriodStart", queryItem.PeriodStart)
                                            .ColumnValue("PeriodEnd", queryItem.PeriodEnd)
                                            .EndInsert()
                                            )
                            .AddQuery().Select()
                                        .From(TABLE_NAME, "au")
                                        .Join().Join(RDBJoinType.Inner, tempTableQuery, "queryTable")
                                                    .And()
                                                        .EqualsCondition("au", "AccountTypeID", "queryTable", "AccountTypeID")
                                                        .EqualsCondition("au", "AccountID", "queryTable", "AccountID")
                                                        .EqualsCondition("au", "TransactionTypeID", "queryTable", "TransactionTypeID")
                                                        .CompareCondition("PeriodStart", RDBCompareConditionOperator.GEq, new RDBColumnExpression { TableAlias = "queryTable", ColumnName = "PeriodStart" })
                                                        .CompareCondition("PeriodEnd", RDBCompareConditionOperator.LEq, new RDBColumnExpression { TableAlias = "queryTable", ColumnName = "PeriodEnd" })
                                                    .EndAnd()
                                        .EndJoin()
                                        .SelectColumns().AllTableColumns("au").EndColumns()
                                        .EndSelect()
                           .EndBatchQuery()
                           .GetItems(AccountUsageMapper);
        }

        public IEnumerable<AccountUsage> GetOverridenAccountUsagesByDeletedTransactionIds(IEnumerable<long> deletedTransactionIds)
        {
            return new RDBQueryContext(GetDataProvider())
                        .Select()
                        .From(TABLE_NAME, "au")
                        .Join()
                            .JoinSelect(RDBJoinType.Inner, "usageOverride")
                                .From(AccountUsageOverrideDataManager.TABLE_NAME, "usageOverride")
                                .Where().ListCondition("OverriddenByTransactionID", RDBListConditionOperator.IN, deletedTransactionIds)
                                .SelectColumns().Columns("AccountTypeID", "AccountID", "TransactionTypeID", "PeriodStart", "PeriodEnd").EndColumns()
                                .EndSelect()
                            .And()
                                .EqualsCondition("au", "AccountTypeID", "usageOverride", "AccountTypeID")
                                .EqualsCondition("au", "AccountID", "usageOverride", "AccountID")
                                .EqualsCondition("au", "TransactionTypeID", "usageOverride", "TransactionTypeID")
                                .CompareCondition("PeriodStart", RDBCompareConditionOperator.GEq, new RDBColumnExpression { TableAlias = "usageOverride", ColumnName = "PeriodStart" })
                                .CompareCondition("PeriodEnd", RDBCompareConditionOperator.LEq, new RDBColumnExpression { TableAlias = "usageOverride", ColumnName = "PeriodEnd" })
                            .EndAnd()
                        .EndJoin()
                        .SelectColumns().AllTableColumns("au").EndColumns()
                        .EndSelect()
                        .GetItems(AccountUsageMapper);                        
        }

        public AccountUsage GetLastAccountUsage(Guid accountTypeId, string accountId)
        {
            return new RDBQueryContext(GetDataProvider())
                        .Select()
                        .From(TABLE_NAME, "au", 1)
                        .Where().And()
                                    .EqualsCondition("AccountTypeID", accountTypeId)
                                    .EqualsCondition("AccountID", accountId)
                                    .ConditionIfColumnNotNull("IsOverridden").EqualsCondition("IsOverridden", false)
                                .EndAnd()
                        .SelectColumns().AllTableColumns("au").EndColumns()
                        .Sort().ByColumn("PeriodEnd", RDBSortDirection.DESC).EndSort()
                        .EndSelect()
                        .GetItem(AccountUsageMapper);
        }

        public IEnumerable<AccountUsage> GetAccountUsagesByAccountIds(Guid accountTypeId, List<Guid> transactionTypeIds, List<string> accountIds)
        {
            return new RDBQueryContext(GetDataProvider())
                        .Select()
                        .From(TABLE_NAME, "au")
                        .Where().And()
                                    .EqualsCondition("AccountTypeID", accountTypeId)
                                    .ConditionIf(() => accountIds != null && accountIds.Count() > 0, ctx => ctx.ListCondition("AccountID", RDBListConditionOperator.IN, accountIds))
                                    .ConditionIf(() => transactionTypeIds != null && transactionTypeIds.Count() > 0, ctx => ctx.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, transactionTypeIds))
                                    .ConditionIfColumnNotNull("IsOverridden").EqualsCondition("IsOverridden", false)
                                .EndAnd()
                        .SelectColumns().AllTableColumns("au").EndColumns()
                        .EndSelect()
                        .GetItems(AccountUsageMapper);
        }

        public IEnumerable<AccountUsage> GetAccountUsagesByTransactionTypes(Guid accountTypeId, List<AccountUsageByTime> accountUsagesByTime, List<Guid> transactionTypeIds)
        {
            var tempTableColumns = new Dictionary<string, RDBTableColumnDefinition>();
            tempTableColumns.Add("AccountID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            tempTableColumns.Add("PeriodEnd", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            var tempTableQuery = new RDBTempTableQuery(tempTableColumns);
            return new RDBQueryContext(GetDataProvider())
                        .StartBatchQuery()
                            .AddQuery().CreateTempTable(tempTableQuery)
                            .Foreach(accountUsagesByTime, (usageByTime, ctx) =>
                                        ctx.AddQuery()
                                            .Insert()
                                            .IntoTable(tempTableQuery)
                                            .ColumnValue("AccountID", usageByTime.AccountId)
                                            .ColumnValue("PeriodEnd", usageByTime.EndPeriod)
                                            .EndInsert())
                            .AddQuery().Select()
                                        .From(TABLE_NAME, "au")
                                        .Join().Join(RDBJoinType.Inner, tempTableQuery, "queryTable")
                                                .And()
                                                    .EqualsCondition("au", "AccountID", "queryTable", "AccountID")
                                                    .CompareCondition("PeriodEnd", RDBCompareConditionOperator.G, new RDBColumnExpression { TableAlias = "queryTable", ColumnName = "PeriodEnd" })
                                                .EndAnd()
                                        .EndJoin()
                                        .Where().And()
                                                    .EqualsCondition("AccountTypeID", accountTypeId)
                                                    .ConditionIf(() => transactionTypeIds != null && transactionTypeIds.Count() > 0, ctx => ctx.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, transactionTypeIds))
                                                    .ConditionIfColumnNotNull("IsOverridden").EqualsCondition("IsOverridden", false)
                                                .EndAnd()
                                        .SelectColumns().AllTableColumns("au").EndColumns()
                                       .EndSelect()
                            .EndBatchQuery()
                            .GetItems(AccountUsageMapper);
        }

        #endregion
    }
}

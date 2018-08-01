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
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au");
            selectQuery.SelectColumns().Columns("ID", "AccountID", "TransactionTypeID", "IsOverridden");

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            whereAndCondition.EqualsCondition("PeriodStart", periodStart);
            whereAndCondition.EqualsCondition("TransactionTypeID", transactionTypeId);

            return queryContext.GetItems(AccountUsageInfoMapper);
        }

        public AccountUsageInfo TryAddAccountUsageAndGet(Guid accountTypeId, Guid transactionTypeId, string accountId, DateTime periodStart, DateTime periodEnd, int currencyId, decimal usageBalance)
        {
            var accountUsageInfo = new AccountUsageInfo
            {
                AccountId = accountId,
                TransactionTypeId = transactionTypeId
            };
            bool accountUsageFound = false;

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au");
            selectQuery.SelectColumns().Columns("ID", "IsOverridden");

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            whereAndCondition.EqualsCondition("AccountID", accountId);
            whereAndCondition.EqualsCondition("TransactionTypeID", transactionTypeId);
            whereAndCondition.EqualsCondition("PeriodStart", periodStart);

            queryContext.ExecuteReader(reader =>
            {
                if (reader.Read())
                {
                    accountUsageFound = true;
                    accountUsageInfo.AccountUsageId = reader.GetLong("ID");
                    accountUsageInfo.IsOverridden = reader.GetBooleanWithNullHandling("IsOverridden");
                }
            });

            if (!accountUsageFound)
            {
                AccountUsageOverrideDataManager accountUsageOverrideDataManager = new AccountUsageOverrideDataManager();
                accountUsageInfo.IsOverridden = accountUsageOverrideDataManager.IsAccountOverriddenInPeriod(accountTypeId, transactionTypeId, accountId, periodStart, periodEnd);                

                queryContext = new RDBQueryContext(GetDataProvider());
                var insertQuery = queryContext.AddInsertQuery();
                insertQuery.IntoTable(TABLE_NAME);
                insertQuery.GenerateIdAndAssignToParameter("AccountUsageId");

                insertQuery.ColumnValue("AccountTypeID", accountTypeId);
                insertQuery.ColumnValue("AccountID", accountId);
                insertQuery.ColumnValue("TransactionTypeID", transactionTypeId);
                insertQuery.ColumnValue("CurrencyId", currencyId);
                insertQuery.ColumnValue("PeriodStart", periodStart);
                insertQuery.ColumnValue("PeriodEnd", periodEnd);
                insertQuery.ColumnValue("UsageBalance", usageBalance);
                insertQuery.ColumnValue("IsOverridden", accountUsageInfo.IsOverridden);
                if (accountUsageInfo.IsOverridden)
                    insertQuery.ColumnValue("OverriddenAmount", 0);

                accountUsageInfo.AccountUsageId = queryContext.ExecuteScalar().LongValue;
            }
            return accountUsageInfo;
        }

        public IEnumerable<AccountUsage> GetAccountUsageForSpecificPeriodByAccountIds(Guid accountTypeId, Guid transactionTypeId, DateTime datePeriod, List<string> accountIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au");
            selectQuery.SelectColumns().AllTableColumns("au");

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            if (accountIds != null && accountIds.Count > 0)
                whereAndCondition.ListCondition("AccountID", RDBListConditionOperator.IN, accountIds);
            whereAndCondition.EqualsCondition("PeriodStart", datePeriod);
            whereAndCondition.EqualsCondition("TransactionTypeID", transactionTypeId);

            return queryContext.GetItems(AccountUsageMapper);
        }

        public List<AccountUsage> GetAccountUsageErrorData(Guid accountTypeId, Guid transactionTypeId, Guid correctionProcessId, DateTime periodDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au");
            selectQuery.SelectColumns().AllTableColumns("au");

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            whereAndCondition.EqualsCondition("PeriodStart", periodDate);
            whereAndCondition.EqualsCondition("TransactionTypeID", transactionTypeId);
            whereAndCondition.ConditionIfColumnNotNull("CorrectionProcessID").CompareCondition("CorrectionProcessID", RDBCompareConditionOperator.NEq, correctionProcessId);

            return queryContext.GetItems(AccountUsageMapper);
        }

        public IEnumerable<AccountUsage> GetAccountUsageForBillingTransactions(Guid accountTypeId, List<Guid> transactionTypeIds, List<string> accountIds, DateTime fromTime, DateTime? toTime, Vanrise.Entities.VRAccountStatus? status, DateTime? effectiveDate, bool? isEffectiveInFuture)
        {
            LiveBalanceDataManager liveBalanceDataManager = new LiveBalanceDataManager();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au");
            selectQuery.SelectColumns().AllTableColumns("au");

            var joinContext = selectQuery.Join();
            liveBalanceDataManager.JoinLiveBalance(joinContext, "lb", "au");

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            if (accountIds != null && accountIds.Count > 0)
                whereAndCondition.ListCondition("AccountID", RDBListConditionOperator.IN, accountIds);
            if (transactionTypeIds != null && transactionTypeIds.Count > 0)
                whereAndCondition.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, transactionTypeIds);
            whereAndCondition.CompareCondition("PeriodStart", RDBCompareConditionOperator.GEq, fromTime);
            if (toTime.HasValue)
                whereAndCondition.CompareCondition("PeriodStart", RDBCompareConditionOperator.LEq, toTime.Value);
            whereAndCondition.ConditionIfColumnNotNull("IsOverridden").EqualsCondition("IsOverridden", false);
            liveBalanceDataManager.AddLiveBalanceActiveAndEffectiveCondition(whereAndCondition, "lb", status, effectiveDate, isEffectiveInFuture);

            return queryContext.GetItems(AccountUsageMapper);
        }

        public IEnumerable<AccountUsage> GetAccountUsagesByAccount(Guid accountTypeId, string accountId, Vanrise.Entities.VRAccountStatus? status, DateTime? effectiveDate, bool? isEffectiveInFuture)
        {
            LiveBalanceDataManager liveBalanceDataManager = new LiveBalanceDataManager();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au");
            selectQuery.SelectColumns().AllTableColumns("au");

            var joinContext = selectQuery.Join();
            liveBalanceDataManager.JoinLiveBalance(joinContext, "lb", "au");

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            whereAndCondition.EqualsCondition("AccountID", accountId);
            whereAndCondition.ConditionIfColumnNotNull("IsOverridden").EqualsCondition("IsOverridden", false);
            liveBalanceDataManager.AddLiveBalanceActiveAndEffectiveCondition(whereAndCondition, "lb", status, effectiveDate, isEffectiveInFuture);

            return queryContext.GetItems(AccountUsageMapper);
        }

        public IEnumerable<AccountUsage> GetAccountUsagesByTransactionAccountUsageQueries(IEnumerable<TransactionAccountUsageQuery> transactionAccountUsageQueries)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableColumns = new Dictionary<string, RDBTableColumnDefinition>();
            tempTableColumns.Add("TransactionID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            tempTableColumns.Add("TransactionTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            tempTableColumns.Add("AccountTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            tempTableColumns.Add("AccountID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            tempTableColumns.Add("PeriodStart", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            tempTableColumns.Add("PeriodEnd", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            
            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumns(tempTableColumns);             

            foreach (var queryItem in transactionAccountUsageQueries)
            {
                var insertQuery = queryContext.AddInsertQuery();
                insertQuery.IntoTable(tempTableQuery);
                insertQuery.ColumnValue("TransactionID", queryItem.TransactionId);
                insertQuery.ColumnValue("TransactionTypeID", queryItem.TransactionTypeId);
                insertQuery.ColumnValue("AccountTypeID", queryItem.AccountTypeId);
                insertQuery.ColumnValue("AccountID", queryItem.AccountId);
                insertQuery.ColumnValue("PeriodStart", queryItem.PeriodStart);
                insertQuery.ColumnValue("PeriodEnd", queryItem.PeriodEnd);
            }

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au");
            selectQuery.SelectColumns().AllTableColumns("au");
            
            selectQuery.Join().Join(RDBJoinType.Inner, tempTableQuery, "queryTable");

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("au", "AccountTypeID", "queryTable", "AccountTypeID");
            whereAndCondition.EqualsCondition("au", "AccountID", "queryTable", "AccountID");
            whereAndCondition.EqualsCondition("au", "TransactionTypeID", "queryTable", "TransactionTypeID");
            whereAndCondition.CompareCondition("PeriodStart", RDBCompareConditionOperator.GEq, new RDBColumnExpression { TableAlias = "queryTable", ColumnName = "PeriodStart" });
            whereAndCondition.CompareCondition("PeriodEnd", RDBCompareConditionOperator.LEq, new RDBColumnExpression { TableAlias = "queryTable", ColumnName = "PeriodEnd" });

            return queryContext.GetItems(AccountUsageMapper);
        }

        public IEnumerable<AccountUsage> GetOverridenAccountUsagesByDeletedTransactionIds(IEnumerable<long> deletedTransactionIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au");
            selectQuery.SelectColumns().AllTableColumns("au");

            var joinContext = selectQuery.Join();
            var joinSelectQuery = joinContext.JoinSelect(RDBJoinType.Inner, "usageOverride");
            joinSelectQuery.From(AccountUsageOverrideDataManager.TABLE_NAME, "usageOverride");
            joinSelectQuery.SelectColumns().Columns("AccountTypeID", "AccountID", "TransactionTypeID", "PeriodStart", "PeriodEnd");

            joinSelectQuery.Where().ListCondition("OverriddenByTransactionID", RDBListConditionOperator.IN, deletedTransactionIds);

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("au", "AccountTypeID", "usageOverride", "AccountTypeID");
            whereAndCondition.EqualsCondition("au", "AccountID", "usageOverride", "AccountID");
            whereAndCondition.EqualsCondition("au", "TransactionTypeID", "usageOverride", "TransactionTypeID");
            whereAndCondition.CompareCondition("PeriodStart", RDBCompareConditionOperator.GEq, new RDBColumnExpression { TableAlias = "usageOverride", ColumnName = "PeriodStart" });
            whereAndCondition.CompareCondition("PeriodEnd", RDBCompareConditionOperator.LEq, new RDBColumnExpression { TableAlias = "usageOverride", ColumnName = "PeriodEnd" });

            return queryContext.GetItems(AccountUsageMapper);                 
        }

        public AccountUsage GetLastAccountUsage(Guid accountTypeId, string accountId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au", 1);
            selectQuery.SelectColumns().AllTableColumns("au");

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            whereAndCondition.EqualsCondition("AccountID", accountId);
            whereAndCondition.ConditionIfColumnNotNull("IsOverridden").EqualsCondition("IsOverridden", false);

            selectQuery.Sort().ByColumn("PeriodEnd", RDBSortDirection.DESC);

            return queryContext.GetItem(AccountUsageMapper);
        }

        public IEnumerable<AccountUsage> GetAccountUsagesByAccountIds(Guid accountTypeId, List<Guid> transactionTypeIds, List<string> accountIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au");
            selectQuery.SelectColumns().AllTableColumns("au");

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            if (accountIds != null && accountIds.Count() > 0)
                whereAndCondition.ListCondition("AccountID", RDBListConditionOperator.IN, accountIds);
            if (transactionTypeIds != null && transactionTypeIds.Count() > 0)
                whereAndCondition.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, transactionTypeIds);
            whereAndCondition.ConditionIfColumnNotNull("IsOverridden").EqualsCondition("IsOverridden", false);

            return queryContext.GetItems(AccountUsageMapper);
        }

        public IEnumerable<AccountUsage> GetAccountUsagesByTransactionTypes(Guid accountTypeId, List<AccountUsageByTime> accountUsagesByTime, List<Guid> transactionTypeIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableColumns = new Dictionary<string, RDBTableColumnDefinition>();
            tempTableColumns.Add("AccountID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            tempTableColumns.Add("PeriodEnd", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumns(tempTableColumns);

            foreach (var usageByTime in accountUsagesByTime)
            {
                var insertQuery = queryContext.AddInsertQuery();
                insertQuery.IntoTable(tempTableQuery);
                insertQuery.ColumnValue("AccountID", usageByTime.AccountId);
                insertQuery.ColumnValue("PeriodEnd", usageByTime.EndPeriod);
            }

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au");
            selectQuery.SelectColumns().AllTableColumns("au");

            var joinContext = selectQuery.Join();
            var joinAndCondition = joinContext.Join(RDBJoinType.Inner, tempTableQuery, "queryTable").And();
            joinAndCondition.EqualsCondition("au", "AccountID", "queryTable", "AccountID");
            joinAndCondition.CompareCondition("PeriodEnd", RDBCompareConditionOperator.G, new RDBColumnExpression { TableAlias = "queryTable", ColumnName = "PeriodEnd" });

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            if (transactionTypeIds != null && transactionTypeIds.Count() > 0)
                whereAndCondition.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, transactionTypeIds);
            whereAndCondition.ConditionIfColumnNotNull("IsOverridden").EqualsCondition("IsOverridden", false);

            return queryContext.GetItems(AccountUsageMapper);
        }

        #endregion

        public void AddQueryOverrideAccountUsages(RDBQueryContext queryContext, IEnumerable<long> accountUsageIds)
        {
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.ColumnValue("IsOverridden", true);
            updateQuery.ColumnValue("OverriddenAmount", new RDBColumnExpression { ColumnName = "UsageBalance" });
            updateQuery.Where().ListCondition("ID", RDBListConditionOperator.IN, accountUsageIds);
        }

        public void AddQueryRollbackOverridenAccountUsages(RDBQueryContext queryContext, IEnumerable<long> accountUsageIds)
        {
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.ColumnValue("IsOverridden", new RDBNullExpression());
            updateQuery.ColumnValue("OverriddenAmount", new RDBNullExpression());
            updateQuery.Where().ListCondition("ID", RDBListConditionOperator.IN, accountUsageIds);
        }

        public void AddQueryUpdateAccountUsageFromBalanceUsageQueue(RDBQueryContext queryContext, IEnumerable<AccountUsageToUpdate> accountsUsageToUpdate, Guid? correctionProcessId)
        {
            var tempTableColumns = new Dictionary<string, RDBTableColumnDefinition> 
                {
                    {"ID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt}},
                    {"UpdateValue", new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6}}
                };
            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumns(tempTableColumns);

            foreach (var auToUpdate in accountsUsageToUpdate)
            {
                var insertQuery = queryContext.AddInsertQuery();
                insertQuery.IntoTable(tempTableQuery);
                insertQuery.ColumnValue("ID", auToUpdate.AccountUsageId);
                insertQuery.ColumnValue("UpdateValue", auToUpdate.Value);
            }

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var joinContext = updateQuery.Join("au");
            joinContext.JoinOnEqualOtherTableColumn(tempTableQuery, "auToUpdate", "ID", "au", "ID");

            updateQuery.ColumnValue("UsageBalance", new RDBArithmeticExpression
                                     {
                                         Operator = RDBArithmeticExpressionOperator.Add,
                                         Expression1 = new RDBColumnExpression { TableAlias = "au", ColumnName = "UsageBalance", DontAppendTableAlias = true },
                                         Expression2 = new RDBColumnExpression { TableAlias = "auToUpdate", ColumnName = "UpdateValue" }
                                     });
            if(correctionProcessId.HasValue)
                updateQuery.ColumnValue("CorrectionProcessID", correctionProcessId.Value);
        }
    }
}

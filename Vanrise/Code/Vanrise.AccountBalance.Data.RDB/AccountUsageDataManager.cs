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

            var where = selectQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            where.EqualsCondition("PeriodStart").Value(periodStart);
            where.EqualsCondition("TransactionTypeID").Value(transactionTypeId);

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

            var where = selectQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            where.EqualsCondition("AccountID").Value(accountId);
            where.EqualsCondition("TransactionTypeID").Value(transactionTypeId);
            where.EqualsCondition("PeriodStart").Value(periodStart);

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

                insertQuery.Column("AccountTypeID").Value(accountTypeId);
                insertQuery.Column("AccountID").Value(accountId);
                insertQuery.Column("TransactionTypeID").Value(transactionTypeId);
                insertQuery.Column("CurrencyId").Value(currencyId);
                insertQuery.Column("PeriodStart").Value(periodStart);
                insertQuery.Column("PeriodEnd").Value(periodEnd);
                insertQuery.Column("UsageBalance").Value(usageBalance);
                insertQuery.Column("IsOverridden").Value(accountUsageInfo.IsOverridden);
                if (accountUsageInfo.IsOverridden)
                    insertQuery.Column("OverriddenAmount").Value(0);

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

            var where = selectQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            if (accountIds != null && accountIds.Count > 0)
                where.ListCondition("AccountID", RDBListConditionOperator.IN, accountIds);
            where.EqualsCondition("PeriodStart").Value(datePeriod);
            where.EqualsCondition("TransactionTypeID").Value(transactionTypeId);

            return queryContext.GetItems(AccountUsageMapper);
        }

        public List<AccountUsage> GetAccountUsageErrorData(Guid accountTypeId, Guid transactionTypeId, Guid correctionProcessId, DateTime periodDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au");
            selectQuery.SelectColumns().AllTableColumns("au");

            var where = selectQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            where.EqualsCondition("PeriodStart").Value(periodDate);
            where.EqualsCondition("TransactionTypeID").Value(transactionTypeId);
            where.ConditionIfColumnNotNull("CorrectionProcessID").CompareCondition("CorrectionProcessID", RDBCompareConditionOperator.NEq).Value(correctionProcessId);

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

            var where = selectQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            if (accountIds != null && accountIds.Count > 0)
                where.ListCondition("AccountID", RDBListConditionOperator.IN, accountIds);
            if (transactionTypeIds != null && transactionTypeIds.Count > 0)
                where.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, transactionTypeIds);
            where.CompareCondition("PeriodStart", RDBCompareConditionOperator.GEq).Value(fromTime);
            if (toTime.HasValue)
                where.CompareCondition("PeriodStart", RDBCompareConditionOperator.LEq).Value(toTime.Value);
            where.ConditionIfColumnNotNull("IsOverridden").EqualsCondition("IsOverridden").Value(false);
            liveBalanceDataManager.AddLiveBalanceActiveAndEffectiveCondition(where, "lb", status, effectiveDate, isEffectiveInFuture);

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

            var where = selectQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            where.EqualsCondition("AccountID").Value(accountId);
            where.ConditionIfColumnNotNull("IsOverridden").EqualsCondition("IsOverridden").Value(false);
            liveBalanceDataManager.AddLiveBalanceActiveAndEffectiveCondition(where, "lb", status, effectiveDate, isEffectiveInFuture);

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
                insertQuery.Column("TransactionID").Value(queryItem.TransactionId);
                insertQuery.Column("TransactionTypeID").Value(queryItem.TransactionTypeId);
                insertQuery.Column("AccountTypeID").Value(queryItem.AccountTypeId);
                insertQuery.Column("AccountID").Value(queryItem.AccountId);
                insertQuery.Column("PeriodStart").Value(queryItem.PeriodStart);
                insertQuery.Column("PeriodEnd").Value(queryItem.PeriodEnd);
            }

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au");
            selectQuery.SelectColumns().AllTableColumns("au");
            
            var joinCondition = selectQuery.Join().Join(tempTableQuery, "queryTable").On();

            joinCondition.EqualsCondition("au", "AccountTypeID", "queryTable", "AccountTypeID");
            joinCondition.EqualsCondition("au", "AccountID", "queryTable", "AccountID");
            joinCondition.EqualsCondition("au", "TransactionTypeID", "queryTable", "TransactionTypeID");
            joinCondition.CompareCondition("PeriodStart", RDBCompareConditionOperator.GEq).Column("queryTable", "PeriodStart");
            joinCondition.CompareCondition("PeriodEnd", RDBCompareConditionOperator.LEq).Column("queryTable", "PeriodEnd");

            return queryContext.GetItems(AccountUsageMapper);
        }

        public IEnumerable<AccountUsage> GetOverridenAccountUsagesByDeletedTransactionIds(IEnumerable<long> deletedTransactionIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au");
            selectQuery.SelectColumns().AllTableColumns("au");

            var joinContext = selectQuery.Join();
            var joinSelectContext = joinContext.JoinSelect("usageOverride");

            var joinSelectQuery = joinSelectContext.SelectQuery();
            joinSelectQuery.From(AccountUsageOverrideDataManager.TABLE_NAME, "usageOverride");
            joinSelectQuery.SelectColumns().Columns("AccountTypeID", "AccountID", "TransactionTypeID", "PeriodStart", "PeriodEnd");
            joinSelectQuery.Where().ListCondition("OverriddenByTransactionID", RDBListConditionOperator.IN, deletedTransactionIds);

            var joinSelectCondition = joinSelectContext.On();
            joinSelectCondition.EqualsCondition("au", "AccountTypeID", "usageOverride", "AccountTypeID");
            joinSelectCondition.EqualsCondition("au", "AccountID", "usageOverride", "AccountID");
            joinSelectCondition.EqualsCondition("au", "TransactionTypeID", "usageOverride", "TransactionTypeID");
            joinSelectCondition.CompareCondition("PeriodStart", RDBCompareConditionOperator.GEq).Column("usageOverride", "PeriodStart");
            joinSelectCondition.CompareCondition("PeriodEnd", RDBCompareConditionOperator.LEq).Column("usageOverride", "PeriodEnd");
            
            return queryContext.GetItems(AccountUsageMapper);                 
        }

        public AccountUsage GetLastAccountUsage(Guid accountTypeId, string accountId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au", 1);
            selectQuery.SelectColumns().AllTableColumns("au");

            var where = selectQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            where.EqualsCondition("AccountID").Value(accountId);
            where.ConditionIfColumnNotNull("IsOverridden").EqualsCondition("IsOverridden").Value(false);

            selectQuery.Sort().ByColumn("PeriodEnd", RDBSortDirection.DESC);

            return queryContext.GetItem(AccountUsageMapper);
        }

        public IEnumerable<AccountUsage> GetAccountUsagesByAccountIds(Guid accountTypeId, List<Guid> transactionTypeIds, List<string> accountIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au");
            selectQuery.SelectColumns().AllTableColumns("au");

            var where = selectQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            if (accountIds != null && accountIds.Count() > 0)
                where.ListCondition("AccountID", RDBListConditionOperator.IN, accountIds);
            if (transactionTypeIds != null && transactionTypeIds.Count() > 0)
                where.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, transactionTypeIds);
            where.ConditionIfColumnNotNull("IsOverridden").EqualsCondition("IsOverridden").Value(false);

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
                insertQuery.Column("AccountID").Value(usageByTime.AccountId);
                insertQuery.Column("PeriodEnd").Value(usageByTime.EndPeriod);
            }

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au");
            selectQuery.SelectColumns().AllTableColumns("au");

            var joinContext = selectQuery.Join();
            var joinCondition = joinContext.Join(tempTableQuery, "queryTable").On();
            joinCondition.EqualsCondition("au", "AccountID", "queryTable", "AccountID");
            joinCondition.CompareCondition("PeriodEnd", RDBCompareConditionOperator.G).Column("queryTable", "PeriodEnd");

            var where = selectQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            if (transactionTypeIds != null && transactionTypeIds.Count() > 0)
                where.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, transactionTypeIds);
            where.ConditionIfColumnNotNull("IsOverridden").EqualsCondition("IsOverridden").Value(false);

            return queryContext.GetItems(AccountUsageMapper);
        }

        #endregion

        public void AddQueryOverrideAccountUsages(RDBQueryContext queryContext, IEnumerable<long> accountUsageIds)
        {
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column("IsOverridden").Value(true);
            updateQuery.Column("OverriddenAmount").Column("UsageBalance");
            updateQuery.Where().ListCondition("ID", RDBListConditionOperator.IN, accountUsageIds);
        }

        public void AddQueryRollbackOverridenAccountUsages(RDBQueryContext queryContext, IEnumerable<long> accountUsageIds)
        {
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column("IsOverridden").Null();
            updateQuery.Column("OverriddenAmount").Null();
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
                insertQuery.Column("ID").Value(auToUpdate.AccountUsageId);
                insertQuery.Column("UpdateValue").Value(auToUpdate.Value);
            }

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var joinContext = updateQuery.Join("au");
            joinContext.JoinOnEqualOtherTableColumn(tempTableQuery, "auToUpdate", "ID", "au", "ID");

            var usageBalanceExpressionContext = updateQuery.Column("UsageBalance").ArithmeticExpression(RDBArithmeticExpressionOperator.Add);
            usageBalanceExpressionContext.Expression1().Column("au", "UsageBalance", true);
            usageBalanceExpressionContext.Expression2().Column("auToUpdate", "UpdateValue");
            if(correctionProcessId.HasValue)
                updateQuery.Column("CorrectionProcessID").Value(correctionProcessId.Value);
        }
    }
}

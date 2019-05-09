﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Data.RDB;
using Vanrise.Entities;
namespace Vanrise.AccountBalance.Data.RDB
{
    public class AccountUsageDataManager : IAccountUsageDataManager
    {
        static string TABLE_NAME = "VR_AccountBalance_AccountUsage";
        const string COL_ID = "ID";
        const string COL_AccountTypeID = "AccountTypeID";
        const string COL_TransactionTypeID = "TransactionTypeID";
        const string COL_AccountID = "AccountID";
        const string COL_CurrencyId = "CurrencyId";
        const string COL_PeriodStart = "PeriodStart";
        const string COL_PeriodEnd = "PeriodEnd";
        const string COL_UsageBalance = "UsageBalance";
        const string COL_IsOverridden = "IsOverridden";
        const string COL_OverriddenAmount = "OverriddenAmount";
        const string COL_CorrectionProcessID = "CorrectionProcessID";
        const string COL_CreatedTime = "CreatedTime";


        static AccountUsageDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_AccountTypeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_TransactionTypeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_AccountID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_CurrencyId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_PeriodStart, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_PeriodEnd, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_UsageBalance, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6 });
            columns.Add(COL_IsOverridden, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_OverriddenAmount, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6 });
            columns.Add(COL_CorrectionProcessID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_AccountBalance",
                DBTableName = "AccountUsage",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
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
                AccountUsageId = reader.GetLong(COL_ID),
                AccountId = reader.GetString(COL_AccountID),
                TransactionTypeId = reader.GetGuid(COL_TransactionTypeID),
                AccountTypeId = reader.GetGuid(COL_AccountTypeID),
                PeriodStart = reader.GetDateTime(COL_PeriodStart),
                PeriodEnd = reader.GetDateTime(COL_PeriodEnd),
                UsageBalance = reader.GetDecimal(COL_UsageBalance),
                CurrencyId = reader.GetInt(COL_CurrencyId),
                IsOverriden = reader.GetBoolean(COL_IsOverridden),
                OverridenAmount = reader.GetNullableDecimal(COL_OverriddenAmount),
                CorrectionProcessId = reader.GetNullableGuid(COL_CorrectionProcessID)
            };
        }
        private GroupedAccountUsage GroupedAccountUsageMapper(IRDBDataReader reader)
        {
            return new GroupedAccountUsage
            {
                AccountId = reader.GetString(COL_AccountID),
                TransactionTypeId = reader.GetGuid(COL_TransactionTypeID),
                AccountTypeId = reader.GetGuid(COL_AccountTypeID),
                PeriodEnd = reader.GetDateTime(COL_PeriodEnd),
                UsageBalance = reader.GetDecimal(COL_UsageBalance),
                CurrencyId = reader.GetInt(COL_CurrencyId),
            };
        }
        private AccountUsageInfo AccountUsageInfoMapper(IRDBDataReader reader)
        {
            return new AccountUsageInfo
            {
                AccountUsageId = reader.GetLong(COL_ID),
                AccountId = reader.GetString(COL_AccountID),
                TransactionTypeId = reader.GetGuid(COL_TransactionTypeID),
                IsOverridden = reader.GetBoolean(COL_IsOverridden),
            };
        }

        #endregion

        #region IAccountUsageDataManager

        public IEnumerable<AccountUsageInfo> GetAccountsUsageInfoByPeriod(Guid accountTypeId, DateTime periodStart, Guid transactionTypeId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au", null, true);
            selectQuery.SelectColumns().Columns(COL_ID, COL_AccountID, COL_TransactionTypeID, COL_IsOverridden);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            where.EqualsCondition(COL_PeriodStart).Value(periodStart);
            where.EqualsCondition(COL_TransactionTypeID).Value(transactionTypeId);

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
            selectQuery.SelectColumns().Columns(COL_ID, COL_IsOverridden);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            where.EqualsCondition(COL_AccountID).Value(accountId);
            where.EqualsCondition(COL_TransactionTypeID).Value(transactionTypeId);
            where.EqualsCondition(COL_PeriodStart).Value(periodStart);

            queryContext.ExecuteReader(reader =>
            {
                if (reader.Read())
                {
                    accountUsageFound = true;
                    accountUsageInfo.AccountUsageId = reader.GetLong(COL_ID);
                    //accountUsageInfo.IsOverridden = reader.GetBoolean(COL_IsOverridden);
                }
            });

            AccountUsageOverrideDataManager accountUsageOverrideDataManager = new AccountUsageOverrideDataManager();
            accountUsageInfo.IsOverridden = accountUsageOverrideDataManager.IsAccountOverriddenInPeriod(accountTypeId, transactionTypeId, accountId, periodStart, periodEnd);

            if (!accountUsageFound)
            {
                

                queryContext = new RDBQueryContext(GetDataProvider());
                var insertQuery = queryContext.AddInsertQuery();
                insertQuery.IntoTable(TABLE_NAME);
                insertQuery.AddSelectGeneratedId();

                insertQuery.Column(COL_AccountTypeID).Value(accountTypeId);
                insertQuery.Column(COL_AccountID).Value(accountId);
                insertQuery.Column(COL_TransactionTypeID).Value(transactionTypeId);
                insertQuery.Column(COL_CurrencyId).Value(currencyId);
                insertQuery.Column(COL_PeriodStart).Value(periodStart);
                insertQuery.Column(COL_PeriodEnd).Value(periodEnd);
                insertQuery.Column(COL_UsageBalance).Value(usageBalance);
                if (accountUsageInfo.IsOverridden)
                {
                    insertQuery.Column(COL_IsOverridden).Value(true);
                    insertQuery.Column(COL_OverriddenAmount).Value(0);
                }

                accountUsageInfo.AccountUsageId = queryContext.ExecuteScalar().LongValue;
            }
            return accountUsageInfo;
        }

        public IEnumerable<AccountUsage> GetAccountUsageForSpecificPeriodByAccountIds(Guid accountTypeId, Guid transactionTypeId, DateTime datePeriod, List<string> accountIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au", null, true);
            selectQuery.SelectColumns().AllTableColumns("au");

            var where = selectQuery.Where();
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            where.ListCondition(COL_AccountID, RDBListConditionOperator.IN, accountIds);
            where.EqualsCondition(COL_PeriodStart).Value(datePeriod);
            where.EqualsCondition(COL_TransactionTypeID).Value(transactionTypeId);

            return queryContext.GetItems(AccountUsageMapper);
        }

        public List<AccountUsage> GetAccountUsageErrorData(Guid accountTypeId, Guid transactionTypeId, Guid correctionProcessId, DateTime periodDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au");
            selectQuery.SelectColumns().AllTableColumns("au");

            var where = selectQuery.Where();
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            where.EqualsCondition(COL_PeriodStart).Value(periodDate);
            where.EqualsCondition(COL_TransactionTypeID).Value(transactionTypeId);
            where.ConditionIfColumnNotNull(COL_CorrectionProcessID).NotEqualsCondition(COL_CorrectionProcessID).Value(correctionProcessId);

            return queryContext.GetItems(AccountUsageMapper);
        }

        public IEnumerable<AccountUsage> GetAccountUsageForBillingTransactions(Guid accountTypeId, List<Guid> transactionTypeIds, List<string> accountIds, DateTime fromTime, DateTime? toTime, Vanrise.Entities.VRAccountStatus? status, DateTime? effectiveDate, bool? isEffectiveInFuture)
        {
            LiveBalanceDataManager liveBalanceDataManager = new LiveBalanceDataManager();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au", null, true);
            selectQuery.SelectColumns().AllTableColumns("au");

            var joinContext = selectQuery.Join();
            liveBalanceDataManager.JoinLiveBalance(joinContext, "lb", "au", COL_AccountTypeID, COL_AccountID);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            if (accountIds != null)
                where.ListCondition(COL_AccountID, RDBListConditionOperator.IN, accountIds);
            if (transactionTypeIds != null)
                where.ListCondition(COL_TransactionTypeID, RDBListConditionOperator.IN, transactionTypeIds);
            where.GreaterOrEqualCondition(COL_PeriodStart).Value(fromTime);
            if (toTime.HasValue)
                where.LessOrEqualCondition(COL_PeriodStart).Value(toTime.Value);
            where.ConditionIfColumnNotNull(COL_IsOverridden).EqualsCondition(COL_IsOverridden).Value(false);
            liveBalanceDataManager.AddLiveBalanceActiveAndEffectiveCondition(where, "lb", status, effectiveDate, isEffectiveInFuture);

            return queryContext.GetItems(AccountUsageMapper);
        }

        public IEnumerable<AccountUsage> GetAccountUsagesByAccount(Guid accountTypeId, string accountId, Vanrise.Entities.VRAccountStatus? status, DateTime? effectiveDate, bool? isEffectiveInFuture)
        {
            LiveBalanceDataManager liveBalanceDataManager = new LiveBalanceDataManager();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au", null, true);
            selectQuery.SelectColumns().AllTableColumns("au");

            var joinContext = selectQuery.Join();
            liveBalanceDataManager.JoinLiveBalance(joinContext, "lb", "au", COL_AccountTypeID, COL_AccountID);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            where.EqualsCondition(COL_AccountID).Value(accountId);
            where.ConditionIfColumnNotNull(COL_IsOverridden).EqualsCondition(COL_IsOverridden).Value(false);
            liveBalanceDataManager.AddLiveBalanceActiveAndEffectiveCondition(where, "lb", status, effectiveDate, isEffectiveInFuture);

            return queryContext.GetItems(AccountUsageMapper);
        }

        public IEnumerable<AccountUsage> GetAccountUsagesByTransactionAccountUsageQueries(IEnumerable<TransactionAccountUsageQuery> transactionAccountUsageQueries)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(COL_AccountTypeID, RDBDataType.UniqueIdentifier , true);
            tempTableQuery.AddColumn(COL_AccountID, RDBDataType.Varchar, 50, null, true);
            tempTableQuery.AddColumn(COL_TransactionTypeID, RDBDataType.UniqueIdentifier, true);
            tempTableQuery.AddColumn(COL_PeriodStart, RDBDataType.DateTime, true);
            tempTableQuery.AddColumn(COL_PeriodEnd, RDBDataType.DateTime, true);
            string transactionIdColumn = "TransactionID";
            tempTableQuery.AddColumn(transactionIdColumn, RDBDataType.BigInt);

            var insertMultipeRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipeRowsQuery.IntoTable(tempTableQuery);
            
            foreach (var queryItem in transactionAccountUsageQueries)
            {
                var rowContext = insertMultipeRowsQuery.AddRow();
                rowContext.Column(transactionIdColumn).Value(queryItem.TransactionId);
                rowContext.Column(COL_TransactionTypeID).Value(queryItem.TransactionTypeId);
                rowContext.Column(COL_AccountTypeID).Value(queryItem.AccountTypeId);
                rowContext.Column(COL_AccountID).Value(queryItem.AccountId);
                rowContext.Column(COL_PeriodStart).Value(queryItem.PeriodStart);
                rowContext.Column(COL_PeriodEnd).Value(queryItem.PeriodEnd);
            }

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au");
            selectQuery.SelectColumns().AllTableColumns("au");
            
            var joinCondition = selectQuery.Join().Join(tempTableQuery, "queryTable").On();

            joinCondition.EqualsCondition("au", COL_AccountTypeID, "queryTable", COL_AccountTypeID);
            joinCondition.EqualsCondition("au", COL_AccountID, "queryTable", COL_AccountID);
            joinCondition.EqualsCondition("au", COL_TransactionTypeID, "queryTable", COL_TransactionTypeID);
            joinCondition.GreaterOrEqualCondition("au", COL_PeriodStart).Column("queryTable", COL_PeriodStart);
            joinCondition.LessOrEqualCondition("au", COL_PeriodEnd).Column("queryTable", COL_PeriodEnd);

            return queryContext.GetItems(AccountUsageMapper);
        }

        public IEnumerable<AccountUsage> GetOverridenAccountUsagesByDeletedTransactionIds(IEnumerable<long> deletedTransactionIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au");
            selectQuery.SelectColumns().AllTableColumns("au");

            var joinSelectContext = selectQuery.Join().JoinSelect("usageOverride");
            AccountUsageOverrideDataManager accountUsageOverrideDataManager = new AccountUsageOverrideDataManager();
            accountUsageOverrideDataManager.AddSelectAccountUsageOverrideToJoinSelect(joinSelectContext, deletedTransactionIds);

            var joinSelectCondition = joinSelectContext.On();
            joinSelectCondition.EqualsCondition("au", COL_AccountTypeID, "usageOverride", AccountUsageOverrideDataManager.COL_AccountTypeID);
            joinSelectCondition.EqualsCondition("au", COL_AccountID, "usageOverride", AccountUsageOverrideDataManager.COL_AccountID);
            joinSelectCondition.EqualsCondition("au", COL_TransactionTypeID, "usageOverride", AccountUsageOverrideDataManager.COL_TransactionTypeID);
            joinSelectCondition.GreaterOrEqualCondition("au", COL_PeriodStart).Column("usageOverride", AccountUsageOverrideDataManager.COL_PeriodStart);
            joinSelectCondition.LessOrEqualCondition("au", COL_PeriodEnd).Column("usageOverride", AccountUsageOverrideDataManager.COL_PeriodEnd);
            
            return queryContext.GetItems(AccountUsageMapper);                 
        }

        public AccountUsage GetLastAccountUsage(Guid accountTypeId, string accountId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au", 1, true);
            selectQuery.SelectColumns().AllTableColumns("au");

            var where = selectQuery.Where();
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            where.EqualsCondition(COL_AccountID).Value(accountId);
            where.ConditionIfColumnNotNull(COL_IsOverridden).EqualsCondition(COL_IsOverridden).Value(false);

            selectQuery.Sort().ByColumn(COL_PeriodEnd, RDBSortDirection.DESC);

            return queryContext.GetItem(AccountUsageMapper);
        }

        public IEnumerable<AccountUsage> GetAccountUsagesByTransactionTypes(Guid accountTypeId, List<AccountUsageByTime> accountUsagesByTime, List<Guid> transactionTypeIds)
        {
            if (accountUsagesByTime == null || accountUsagesByTime.Count == 0)
                return null;

            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(COL_AccountID, RDBDataType.Varchar, 50, null, true);
            tempTableQuery.AddColumn(COL_PeriodEnd, RDBDataType.DateTime, true);

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(tempTableQuery);

            foreach (var usageByTime in accountUsagesByTime)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(COL_AccountID).Value(usageByTime.AccountId);
                rowContext.Column(COL_PeriodEnd).Value(usageByTime.EndPeriod);
            }

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au", null, true);
            selectQuery.SelectColumns().AllTableColumns("au");

            var joinContext = selectQuery.Join();
            var joinCondition = joinContext.Join(tempTableQuery, "queryTable").On();
            joinCondition.EqualsCondition("au", COL_AccountID, "queryTable", COL_AccountID);
            joinCondition.GreaterThanCondition("au", COL_PeriodEnd).Column("queryTable", COL_PeriodEnd);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            if (transactionTypeIds != null && transactionTypeIds.Count() > 0)
                where.ListCondition(COL_TransactionTypeID, RDBListConditionOperator.IN, transactionTypeIds);
            where.ConditionIfColumnNotNull(COL_IsOverridden).EqualsCondition(COL_IsOverridden).Value(false);

            return queryContext.GetItems(AccountUsageMapper);
        }
        public IEnumerable<GroupedAccountUsage> GetGroupedAccountUsagesByAccountIds(Guid accountTypeId, List<Guid> transactionTypeIds, List<string> accountIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "au", null, true);
            selectQuery.SelectColumns().Columns(COL_AccountTypeID, COL_TransactionTypeID, COL_AccountID, COL_CurrencyId);
            selectQuery.SelectColumns().Expression(COL_PeriodEnd).DateTimePart(RDBDateTimePart.DateOnly).Column(COL_PeriodEnd);
            selectQuery.SelectAggregates().ExpressionAggregate(RDBNonCountAggregateType.SUM, COL_UsageBalance).Column(COL_UsageBalance);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            if (accountIds != null && accountIds.Count() > 0)
                where.ListCondition(COL_AccountID, RDBListConditionOperator.IN, accountIds);
            if (transactionTypeIds != null && transactionTypeIds.Count() > 0)
                where.ListCondition(COL_TransactionTypeID, RDBListConditionOperator.IN, transactionTypeIds);
            where.ConditionIfColumnNotNull(COL_IsOverridden).EqualsCondition(COL_IsOverridden).Value(false);

           
            var selectGrouping = selectQuery.GroupBy();
            var groupingSelect = selectGrouping.Select();
            groupingSelect.Columns(COL_AccountTypeID, COL_TransactionTypeID, COL_AccountID, COL_CurrencyId);
            groupingSelect.Expression(COL_PeriodEnd).DateTimePart(RDBDateTimePart.DateOnly).Column(COL_PeriodEnd);
            return queryContext.GetItems(GroupedAccountUsageMapper);
        }
       
        #endregion

        public void AddQueryOverrideAccountUsages(RDBQueryContext queryContext, IEnumerable<long> accountUsageIds)
        {
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_IsOverridden).Value(true);
            updateQuery.Column(COL_OverriddenAmount).Column(COL_UsageBalance);
            updateQuery.Where().ListCondition(COL_ID, RDBListConditionOperator.IN, accountUsageIds);
        }

        public void AddQueryRollbackOverridenAccountUsages(RDBQueryContext queryContext, IEnumerable<long> accountUsageIds)
        {
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_IsOverridden).Null();
            updateQuery.Column(COL_OverriddenAmount).Null();
            updateQuery.Where().ListCondition(COL_ID, RDBListConditionOperator.IN, accountUsageIds);
        }

        public void AddQueryUpdateAccountUsageFromBalanceUsageQueue(RDBQueryContext queryContext, IEnumerable<AccountUsageToUpdate> accountsUsageToUpdate, Guid? correctionProcessId)
        {
            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(COL_ID, RDBDataType.BigInt, true);
            tempTableQuery.AddColumn("UpdateValue", RDBDataType.Decimal, 20, 6);

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(tempTableQuery);
            
            foreach (var auToUpdate in accountsUsageToUpdate)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(COL_ID).Value(auToUpdate.AccountUsageId);
                rowContext.Column("UpdateValue").Value(auToUpdate.Value);
            }

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var joinContext = updateQuery.Join("au");
            joinContext.JoinOnEqualOtherTableColumn(tempTableQuery, "auToUpdate", COL_ID, "au", COL_ID);

            var usageBalanceExpressionContext = updateQuery.Column(COL_UsageBalance).ArithmeticExpression(RDBArithmeticExpressionOperator.Add);
            usageBalanceExpressionContext.Expression1().Column("au", COL_UsageBalance);
            usageBalanceExpressionContext.Expression2().Column("auToUpdate", "UpdateValue");
            if (correctionProcessId.HasValue)
                updateQuery.Column(COL_CorrectionProcessID).Value(correctionProcessId.Value);
            else
                updateQuery.Column(COL_CorrectionProcessID).Null();
        }

      
    }
}

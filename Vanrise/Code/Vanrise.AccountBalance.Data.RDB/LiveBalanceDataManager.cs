using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using Vanrise.Notification.Entities;

namespace Vanrise.AccountBalance.Data.RDB
{
    public class LiveBalanceDataManager : ILiveBalanceDataManager
    {
        public static string TABLE_NAME = "VR_AccountBalance_LiveBalance";

        static LiveBalanceDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add("ID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("AccountTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("AccountID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add("CurrencyID", new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add("InitialBalance", new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6 });
            columns.Add("CurrentBalance", new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6 });
            columns.Add("NextAlertThreshold", new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6 });
            columns.Add("LastExecutedActionThreshold", new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6 });
            columns.Add("AlertRuleID", new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add("ActiveAlertsInfo", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add("BED", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("EED", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("Status", new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add("IsDeleted", new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add("CreatedTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_AccountBalance",
                DBTableName = "LiveBalance",
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

        private LiveBalance LiveBalanceMapper(IRDBDataReader reader)
        {
            return new LiveBalance
            {
                CurrentBalance = reader.GetDecimal("CurrentBalance"),
                AccountId = reader.GetString("AccountID"),
                AccountTypeId = reader.GetGuid("AccountTypeID"),
                CurrencyId = reader.GetInt("CurrencyID"),
                AlertRuleID = reader.GetIntWithNullHandling("AlertRuleID"),
                InitialBalance = reader.GetDecimal("InitialBalance"),
                NextThreshold = reader.GetNullableDecimal("NextAlertThreshold"),
                LastExecutedThreshold = reader.GetNullableDecimal("LastExecutedActionThreshold"),
                LiveBalanceActiveAlertsInfo = Serializer.Deserialize(reader.GetString("ActiveAlertsInfo"), typeof(VRBalanceActiveAlertInfo)) as VRBalanceActiveAlertInfo,
                BED = reader.GetNullableDateTime("BED"),
                EED = reader.GetNullableDateTime("EED"),
                Status = (VRAccountStatus) reader.GetIntWithNullHandling("Status"),
            };
        }

        private Vanrise.AccountBalance.Entities.AccountBalance AccountBalanceMapper(IRDBDataReader reader)
        {
            return new Vanrise.AccountBalance.Entities.AccountBalance
            {
                AccountBalanceId = reader.GetLong("ID"),
                AccountId = reader.GetString("AccountID"),
                AccountTypeId = reader.GetGuid("AccountTypeID"),
                CurrentBalance = reader.GetDecimal("CurrentBalance"),
                CurrencyId = reader.GetInt("CurrencyID"),
                InitialBalance = reader.GetDecimal("InitialBalance"),
                AlertRuleID = reader.GetIntWithNullHandling("AlertRuleID"),
                BED = reader.GetNullableDateTime("BED"),
                EED = reader.GetNullableDateTime("EED"),
                Status = (VRAccountStatus)reader.GetIntWithNullHandling("Status")

            };
        }

        private LiveBalanceAccountInfo LiveBalanceAccountInfoMapper(IRDBDataReader reader)
        {
            return new LiveBalanceAccountInfo
            {
                LiveBalanceId = reader.GetLong("ID"),
                AccountId = reader.GetString("AccountID"),
                CurrencyId = reader.GetInt("CurrencyID"),
                BED = reader.GetNullableDateTime("BED"),
                EED = reader.GetNullableDateTime("EED"),
                Status = (VRAccountStatus)reader.GetIntWithNullHandling("Status")
            };
        }

        #endregion

        #region ILiveBalanceDataManager

        public LiveBalance GetLiveBalance(Guid accountTypeId, string accountId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "lb");
            selectQuery.SelectColumns().AllTableColumns("lb");

            var where = selectQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            where.EqualsCondition("AccountID").Value(accountId);
            where.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted").Value(false);
            
            return queryContext.GetItem(LiveBalanceMapper);
        }

        public void GetLiveBalanceAccounts(Guid accountTypeId, Action<LiveBalance> onLiveBalanceReady)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "lb");
            selectQuery.SelectColumns().AllTableColumns("lb");

            var where = selectQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            where.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted").Value(false);

            queryContext.ExecuteReader(
                reader =>
                {
                    while (reader.Read())
                    {
                        onLiveBalanceReady(LiveBalanceMapper(reader));
                    }
                });
        }

        public IEnumerable<Entities.AccountBalance> GetFilteredAccountBalances(AccountBalanceQuery query)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "lb", query.Top);
            selectQuery.SelectColumns().AllTableColumns("lb");

            var where = selectQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(query.AccountTypeId);
            if (query.AccountsIds != null && query.AccountsIds.Count() > 0)
                where.ListCondition("AccountID", RDBListConditionOperator.IN, query.AccountsIds);
            if (query.Sign != null)
                where.CompareCondition("CurrentBalance", ConvertBalanceCompareSign(query.Sign)).Value(query.Balance);
            AddLiveBalanceActiveAndEffectiveCondition(where, "lb", query.Status, query.EffectiveDate, query.IsEffectiveInFuture);

            var sortContext = selectQuery.Sort();
            sortContext.ByColumn("CurrentBalance", query.OrderBy == "ASC" ? RDBSortDirection.ASC : RDBSortDirection.DESC);

            return queryContext.GetItems(AccountBalanceMapper);
        }

        private RDBCompareConditionOperator ConvertBalanceCompareSign(string sign)
        {
            switch(sign)
            {
                case ">": return RDBCompareConditionOperator.G;
                case ">=": return RDBCompareConditionOperator.GEq;
                case "<": return RDBCompareConditionOperator.L;
                case "<=": return RDBCompareConditionOperator.LEq;
                default: throw new NotSupportedException(String.Format("Sign '{0},", sign));
            }
        }

        public bool UpdateLiveBalancesFromBillingTransactions(IEnumerable<LiveBalanceToUpdate> liveBalancesToUpdate, IEnumerable<long> billingTransactionIds, IEnumerable<long> accountUsageIdsToOverride, IEnumerable<AccountUsageOverride> accountUsageOverrides, IEnumerable<long> overridenUsageIdsToRollback, IEnumerable<long> deletedTransactionIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            BillingTransactionDataManager billingTransactionDataManager = new BillingTransactionDataManager();
            AccountUsageDataManager accountUsageDataManager = new AccountUsageDataManager();
            AccountUsageOverrideDataManager accountUsageOverrideDataManager = new AccountUsageOverrideDataManager();
            
            if (overridenUsageIdsToRollback != null && overridenUsageIdsToRollback.Count() > 0)
                accountUsageDataManager.AddQueryRollbackOverridenAccountUsages(queryContext, overridenUsageIdsToRollback);

            if (accountUsageIdsToOverride != null && accountUsageIdsToOverride.Count() > 0)
                accountUsageDataManager.AddQueryOverrideAccountUsages(queryContext, accountUsageIdsToOverride);

            if (deletedTransactionIds != null && deletedTransactionIds.Count() > 0)
            {
                accountUsageOverrideDataManager.AddQueryDeleteAccountUsageOverrideByTransactionIds(queryContext, deletedTransactionIds);
                billingTransactionDataManager.AddQuerySetBillingTransactionsAsSubtractedFromBalance(queryContext, deletedTransactionIds);
            }

            if (accountUsageOverrides != null && accountUsageOverrides.Count() > 0)
                accountUsageOverrideDataManager.AddQueryInsertAccountUsageOverrides(queryContext, accountUsageOverrides);

            if (billingTransactionIds != null && billingTransactionIds.Count() > 0)
                billingTransactionDataManager.AddQuerySetBillingTransactionsAsBalanceUpdated(queryContext, billingTransactionIds);

            if (liveBalancesToUpdate != null && liveBalancesToUpdate.Count() > 0)
                AddQueryUpdateLiveBalances(queryContext, liveBalancesToUpdate);

            queryContext.ExecuteNonQuery(true);                                        
            return true;
        }

        public IEnumerable<LiveBalanceAccountInfo> GetLiveBalanceAccountsInfo(Guid accountTypeId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "lb");
            selectQuery.SelectColumns().Columns("ID", "AccountID", "CurrencyID", "BED", "EED", "Status");

            var where = selectQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            where.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted").Value(false);

            return queryContext.GetItems(LiveBalanceAccountInfoMapper);
        }

        public LiveBalanceAccountInfo TryAddLiveBalanceAndGet(string accountId, Guid accountTypeId, decimal initialBalance, int currencyId, decimal currentBalance, DateTime? bed, DateTime? eed, VRAccountStatus status, bool isDeleted)
        {
            LiveBalanceAccountInfo liveBalanceInfo = new LiveBalanceAccountInfo
            {
                AccountId = accountId,
                BED = bed,
                EED = eed,
                Status = status
            };
            bool liveBalanceFound = false;
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "lv");
            selectQuery.SelectColumns().Columns("ID", "CurrencyID");

            var where = selectQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            where.EqualsCondition("AccountID").Value(accountId);
            where.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted").Value(false);

            queryContext.ExecuteReader((reader) =>
            {
                if (reader.Read())
                {
                    liveBalanceFound = true;
                    liveBalanceInfo.LiveBalanceId = reader.GetLong("ID");
                    liveBalanceInfo.CurrencyId = reader.GetInt("CurrencyID");
                }
            });

            if(!liveBalanceFound)
            {
                queryContext = new RDBQueryContext(GetDataProvider());
                var insertQuery = queryContext.AddInsertQuery();
                insertQuery.IntoTable(TABLE_NAME);
                insertQuery.GenerateIdAndAssignToParameter("ID");
                insertQuery.Column("AccountTypeID").Value(accountTypeId);
                insertQuery.Column("AccountID").Value(accountId);
                insertQuery.Column("InitialBalance").Value(initialBalance);
                insertQuery.Column("CurrentBalance").Value(currentBalance);
                insertQuery.Column("CurrencyID").Value(currencyId);
                if (bed.HasValue)
                    insertQuery.Column("BED").Value(bed.Value);
                if(eed.HasValue)
                    insertQuery.Column("EED").Value(eed.Value);
                insertQuery.Column("Status").Value((int)status);
                insertQuery.Column("IsDeleted").Value(isDeleted);
                liveBalanceInfo.LiveBalanceId = queryContext.ExecuteScalar().LongValue;
                liveBalanceInfo.CurrencyId = currencyId;
            }
            return liveBalanceInfo;
        }

        public bool UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(long balanceUsageQueueId, IEnumerable<LiveBalanceToUpdate> liveBalancesToUpdate, IEnumerable<AccountUsageToUpdate> accountsUsageToUpdate, Guid? correctionProcessId)
        {
            AccountUsageDataManager accountUsageDataManager = new AccountUsageDataManager();
            BalanceUsageQueueDataManager balanceUsageQueueDataManager = new BalanceUsageQueueDataManager();
            var queryContext = new RDBQueryContext(GetDataProvider());

            if (liveBalancesToUpdate != null && liveBalancesToUpdate.Count() > 0)
                AddQueryUpdateLiveBalances(queryContext, liveBalancesToUpdate);
            if (accountsUsageToUpdate != null && accountsUsageToUpdate.Count() > 0)
                accountUsageDataManager.AddQueryUpdateAccountUsageFromBalanceUsageQueue(queryContext, accountsUsageToUpdate, correctionProcessId);
            balanceUsageQueueDataManager.AddQueryDeleteBalanceUsageQueue(queryContext, balanceUsageQueueId);

            queryContext.ExecuteNonQuery(true);
            return true;
        }

        public void UpdateBalanceRuleInfos(List<LiveBalanceNextThresholdUpdateEntity> updateEntities)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableColumns = new Dictionary<string, RDBTableColumnDefinition>();
            tempTableColumns.Add("AccountTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            tempTableColumns.Add("AccountID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            tempTableColumns.Add("NextAlertThreshold", new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6 });
            tempTableColumns.Add("AlertRuleID", new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            
            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumns(tempTableColumns);

            if (updateEntities != null)
            {
                foreach(var updateEntity in updateEntities)
                {
                    var insertQuery = queryContext.AddInsertQuery();
                    insertQuery.IntoTable(tempTableQuery);
                    insertQuery.Column("AccountTypeID").Value(updateEntity.AccountTypeId);
                    insertQuery.Column("AccountID").Value(updateEntity.AccountId);
                    if (updateEntity.NextAlertThreshold.HasValue)
                        insertQuery.Column("NextAlertThreshold").Value(updateEntity.NextAlertThreshold.Value);
                    if (updateEntity.AlertRuleId.HasValue)
                        insertQuery.Column("AlertRuleID").Value(updateEntity.AlertRuleId.Value);
                }
            }

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            
            var joinContext = updateQuery.Join("lb");
            var joinCondition = joinContext.Join(tempTableQuery, "updateEntities").On();
            joinCondition.EqualsCondition("lb", "AccountTypeID", "updateEntities", "AccountTypeID");
            joinCondition.EqualsCondition("lb", "AccountID", "updateEntities", "AccountID");

            updateQuery.Column("NextAlertThreshold").Column("updateEntities", "NextAlertThreshold");
            updateQuery.Column("AlertRuleID").Column("updateEntities", "AlertRuleID");

            queryContext.ExecuteNonQuery();
        }

        public void UpdateBalanceLastAlertInfos(List<LiveBalanceLastThresholdUpdateEntity> updateEntities)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableColumns = new Dictionary<string, RDBTableColumnDefinition>();
            tempTableColumns.Add("AccountTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            tempTableColumns.Add("AccountID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            tempTableColumns.Add("LastExecutedActionThreshold", new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6 });
            tempTableColumns.Add("ActiveAlertsInfo", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            
            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumns(tempTableColumns);

            if (updateEntities != null)
            {
                foreach (var updateEntity in updateEntities)
                {
                    var insertQuery = queryContext.AddInsertQuery();
                    insertQuery.IntoTable(tempTableQuery);
                    insertQuery.Column("AccountTypeID").Value(updateEntity.AccountTypeId);
                    insertQuery.Column("AccountID").Value(updateEntity.AccountId);
                    if (updateEntity.LastExecutedActionThreshold.HasValue)
                        insertQuery.Column("LastExecutedActionThreshold").Value(updateEntity.LastExecutedActionThreshold.Value);
                    if (updateEntity.ActiveAlertsInfo != null)
                        insertQuery.Column("ActiveAlertsInfo").Value(Serializer.Serialize(updateEntity.ActiveAlertsInfo, true));
                }
            }

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var joinContext = updateQuery.Join("lb");
            var joinCondition = joinContext.Join(tempTableQuery, "updateEntities").On();
            joinCondition.EqualsCondition("lb", "AccountTypeID", "updateEntities", "AccountTypeID");
            joinCondition.EqualsCondition("lb", "AccountID", "updateEntities", "AccountID");

            updateQuery.Column("LastExecutedActionThreshold").Column("updateEntities", "LastExecutedActionThreshold");
            updateQuery.Column("ActiveAlertsInfo").Column("updateEntities", "ActiveAlertsInfo");

            queryContext.ExecuteNonQuery();
        }

        public void GetLiveBalancesToAlert(Guid accountTypeId, Action<LiveBalance> onLiveBalanceReady)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "lb");
            selectQuery.SelectColumns().AllTableColumns("lb");

            var where = selectQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            AddLiveBalanceToAlertCondition(where, "lb");
            where.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted").Value(false);

            queryContext.ExecuteReader(reader =>
                    {
                        while (reader.Read())
                        {
                            onLiveBalanceReady(LiveBalanceMapper(reader));
                        }
                    });
        }

        public void GetLiveBalancesToClearAlert(Guid accountTypeId, Action<LiveBalance> onLiveBalanceReady)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "lb");
            selectQuery.SelectColumns().AllTableColumns("lb");

            var where = selectQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            AddLiveBalanceToClearAlertCondition(where, "lb");
            where.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted").Value(false);

            queryContext.ExecuteReader(reader =>
            {
                while (reader.Read())
                {
                    onLiveBalanceReady(LiveBalanceMapper(reader));
                }
            });
        }

        public bool HasLiveBalancesUpdateData(Guid accountTypeId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "lb", 1);
            selectQuery.SelectColumns().Column("ID");

            var whereOrCondition = selectQuery.Where();
            AddLiveBalanceToAlertCondition(whereOrCondition, "lb");
            AddLiveBalanceToClearAlertCondition(whereOrCondition, "lb");

            return queryContext.ExecuteScalar().NullableLongValue.HasValue;
        }

        public bool CheckIfAccountHasTransactions(Guid accountTypeId, string accountId)
        {
            throw new NotImplementedException();
        }

        public bool TryUpdateLiveBalanceStatus(string accountId, Guid accountTypeId, VRAccountStatus status, bool isDeleted)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column("Status").Value((int)status);
           updateQuery.Column("IsDeleted").Value(isDeleted);

            var where = updateQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            where.EqualsCondition("AccountID").Value(accountId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool TryUpdateLiveBalanceEffectiveDate(string accountId, Guid accountTypeId, DateTime? bed, DateTime? eed)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            if (bed.HasValue)
                updateQuery.Column("BED").Value(bed.Value);
            if (eed.HasValue)
                updateQuery.Column("EED").Value(eed.Value);

            var where = updateQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            where.EqualsCondition("AccountID").Value(accountId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        #endregion

        public void JoinLiveBalance(RDBJoinContext joinContext, string liveBalanceAlias, string originalTableAlias)
        {
            var joinStatement = joinContext.Join(LiveBalanceDataManager.TABLE_NAME, liveBalanceAlias);
            joinStatement.JoinType(RDBJoinType.Left);
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(liveBalanceAlias, "AccountTypeID", originalTableAlias, "AccountTypeID");
            joinCondition.EqualsCondition(liveBalanceAlias, "AccountID", originalTableAlias, "AccountID");                    
        }

        public void AddLiveBalanceActiveAndEffectiveCondition(RDBConditionContext conditionContext, string liveBalanceAlias, VRAccountStatus? accountStatus, DateTime? effectiveDate, bool? isEffectiveInFuture)
        {
            var orCondition = conditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(liveBalanceAlias, "AccountID");
            var andCondition = orCondition.ChildConditionGroup();

            andCondition.ConditionIfColumnNotNull(liveBalanceAlias, "IsDeleted").EqualsCondition(liveBalanceAlias, "IsDeleted").Value(false);
            if (accountStatus.HasValue)
                andCondition.EqualsCondition(liveBalanceAlias, "Status").Value((int)accountStatus.Value);

            if (effectiveDate.HasValue)
            {
                andCondition.ConditionIfColumnNotNull(liveBalanceAlias, "BED").CompareCondition(liveBalanceAlias, "BED", RDBCompareConditionOperator.LEq).Value(effectiveDate.Value);
                andCondition.ConditionIfColumnNotNull(liveBalanceAlias, "EED").CompareCondition(liveBalanceAlias, "EED", RDBCompareConditionOperator.G).Value(effectiveDate.Value);
            }

            if(isEffectiveInFuture.HasValue)
            {
                if(isEffectiveInFuture.Value)
                {
                    andCondition.ConditionIfColumnNotNull(liveBalanceAlias, "EED").CompareCondition(liveBalanceAlias, "EED", RDBCompareConditionOperator.GEq).DateNow();
                }
                else
                {
                    andCondition.NotNullCondition(liveBalanceAlias, "EED");
                    andCondition.CompareCondition(liveBalanceAlias, "EED", RDBCompareConditionOperator.LEq).DateNow();
                }
            }
        }

        private void AddQueryUpdateLiveBalances(RDBQueryContext queryContext, IEnumerable<LiveBalanceToUpdate> liveBalancesToUpdate)
        {
            var tempTableColumns = new Dictionary<string, RDBTableColumnDefinition> 
                {
                    {"ID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt}},
                    {"UpdateValue", new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6}}
                };

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumns(tempTableColumns);

            foreach (var lvToUpdate in liveBalancesToUpdate)
            {
                var insertQuery = queryContext.AddInsertQuery();
                insertQuery.IntoTable(tempTableQuery);
                insertQuery.Column("ID").Value(lvToUpdate.LiveBalanceId);
                insertQuery.Column("UpdateValue").Value(lvToUpdate.Value);
            }
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var joinContext = updateQuery.Join("lv");
            joinContext.JoinOnEqualOtherTableColumn(tempTableQuery, "lvToUpdate", "ID", "lv", "ID");

            var currentBalanceExpressionContext = updateQuery.Column("CurrentBalance").ArithmeticExpression(RDBArithmeticExpressionOperator.Add);
            currentBalanceExpressionContext.Expression1().Column("lv", "CurrentBalance", true);
            currentBalanceExpressionContext.Expression2().Column("lvToUpdate", "UpdateValue");
        }

        private void AddLiveBalanceToAlertCondition(RDBConditionContext conditionContext, string liveBalanceAlias)
        {
            var andCondition = conditionContext.ChildConditionGroup();
            andCondition.CompareCondition("CurrentBalance", RDBCompareConditionOperator.LEq).Column(liveBalanceAlias, "NextAlertThreshold");
            andCondition.ConditionIfColumnNotNull("LastExecutedActionThreshold").CompareCondition("NextAlertThreshold", RDBCompareConditionOperator.L).Column(liveBalanceAlias, "LastExecutedActionThreshold");
        }
        private void AddLiveBalanceToClearAlertCondition(RDBConditionContext conditionContext, string liveBalanceAlias)
        {
            conditionContext.CompareCondition("CurrentBalance", RDBCompareConditionOperator.G).Column(liveBalanceAlias, "LastExecutedActionThreshold");
        }

    }
}
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

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            whereAndCondition.EqualsCondition("AccountID", accountId);
            whereAndCondition.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false);
            
            return queryContext.GetItem(LiveBalanceMapper);
        }

        public void GetLiveBalanceAccounts(Guid accountTypeId, Action<LiveBalance> onLiveBalanceReady)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "lb");
            selectQuery.SelectColumns().AllTableColumns("lb");

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            whereAndCondition.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false);

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

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", query.AccountTypeId);
            if (query.AccountsIds != null && query.AccountsIds.Count() > 0)
                whereAndCondition.ListCondition("AccountID", RDBListConditionOperator.IN, query.AccountsIds);
            if (query.Sign != null)
                whereAndCondition.CompareCondition("CurrentBalance", ConvertBalanceCompareSign(query.Sign), query.Balance);
            AddLiveBalanceActiveAndEffectiveCondition(whereAndCondition, "lb", query.Status, query.EffectiveDate, query.IsEffectiveInFuture);

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

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            whereAndCondition.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false);

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

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            whereAndCondition.EqualsCondition("AccountID", accountId);
            whereAndCondition.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false);

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
                insertQuery.ColumnValue("AccountTypeID", accountTypeId);
                insertQuery.ColumnValue("AccountID", accountId);
                insertQuery.ColumnValue("InitialBalance", initialBalance);
                insertQuery.ColumnValue("CurrentBalance", currentBalance);
                insertQuery.ColumnValue("CurrencyID", currencyId);
                if (bed.HasValue)
                    insertQuery.ColumnValue("BED", bed.Value);
                if(eed.HasValue)
                insertQuery.ColumnValue("EED", eed.Value);
                insertQuery.ColumnValue("Status", (int)status);
                insertQuery.ColumnValue("IsDeleted", isDeleted);
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
                    insertQuery.ColumnValue("AccountTypeID", updateEntity.AccountTypeId);
                    insertQuery.ColumnValue("AccountID", updateEntity.AccountId);
                    if (updateEntity.NextAlertThreshold.HasValue)
                        insertQuery.ColumnValue("NextAlertThreshold", updateEntity.NextAlertThreshold.Value);
                    if (updateEntity.AlertRuleId.HasValue)
                        insertQuery.ColumnValue("AlertRuleID", updateEntity.AlertRuleId.Value);
                }
            }

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            
            var joinContext = updateQuery.Join("lb");
            var joinAndCondition = joinContext.Join(RDBJoinType.Inner, tempTableQuery, "updateEntities").And();
            joinAndCondition.EqualsCondition("lb", "AccountTypeID", "updateEntities", "AccountTypeID");
            joinAndCondition.EqualsCondition("lb", "AccountID", "updateEntities", "AccountID");

            updateQuery.ColumnValue("NextAlertThreshold", new RDBColumnExpression { TableAlias = "updateEntities", ColumnName = "NextAlertThreshold" });
            updateQuery.ColumnValue("AlertRuleID", new RDBColumnExpression { TableAlias = "updateEntities", ColumnName = "AlertRuleID" });

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
                    insertQuery.ColumnValue("AccountTypeID", updateEntity.AccountTypeId);
                    insertQuery.ColumnValue("AccountID", updateEntity.AccountId);
                    if (updateEntity.LastExecutedActionThreshold.HasValue)
                        insertQuery.ColumnValue("LastExecutedActionThreshold", updateEntity.LastExecutedActionThreshold.Value);
                    if (updateEntity.ActiveAlertsInfo != null)
                        insertQuery.ColumnValue("ActiveAlertsInfo", Serializer.Serialize(updateEntity.ActiveAlertsInfo, true));
                }
            }

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var joinContext = updateQuery.Join("lb");
            var joinAndCondition = joinContext.Join(RDBJoinType.Inner, tempTableQuery, "updateEntities").And();
            joinAndCondition.EqualsCondition("lb", "AccountTypeID", "updateEntities", "AccountTypeID");
            joinAndCondition.EqualsCondition("lb", "AccountID", "updateEntities", "AccountID");

            updateQuery.ColumnValue("LastExecutedActionThreshold", new RDBColumnExpression { TableAlias = "updateEntities", ColumnName = "LastExecutedActionThreshold" });
            updateQuery.ColumnValue("ActiveAlertsInfo", new RDBColumnExpression { TableAlias = "updateEntities", ColumnName = "ActiveAlertsInfo" });

            queryContext.ExecuteNonQuery();
        }

        public void GetLiveBalancesToAlert(Guid accountTypeId, Action<LiveBalance> onLiveBalanceReady)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "lb");
            selectQuery.SelectColumns().AllTableColumns("lb");

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            AddLiveBalanceToAlertCondition(whereAndCondition, "lb");
            whereAndCondition.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false);

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

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            AddLiveBalanceToClearAlertCondition(whereAndCondition, "lb");
            whereAndCondition.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false);

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

            var whereOrCondition = selectQuery.Where().And();
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

           updateQuery.ColumnValue("Status", (int)status);
           updateQuery.ColumnValue("IsDeleted", isDeleted);

            var whereAndCondition = updateQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            whereAndCondition.EqualsCondition("AccountID", accountId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool TryUpdateLiveBalanceEffectiveDate(string accountId, Guid accountTypeId, DateTime? bed, DateTime? eed)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            if (bed.HasValue)
                updateQuery.ColumnValue("BED", bed.Value);
            if (eed.HasValue)
                updateQuery.ColumnValue("EED", eed.Value);

            var whereAndCondition = updateQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            whereAndCondition.EqualsCondition("AccountID", accountId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        #endregion

        public void JoinLiveBalance(RDBJoinContext joinContext, string liveBalanceAlias, string originalTableAlias)
        {
            var joinAndCondition = joinContext.Join(RDBJoinType.Left, LiveBalanceDataManager.TABLE_NAME, liveBalanceAlias).And();
            joinAndCondition.EqualsCondition(liveBalanceAlias, "AccountTypeID", originalTableAlias, "AccountTypeID");
            joinAndCondition.EqualsCondition(liveBalanceAlias, "AccountID", originalTableAlias, "AccountID");                    
        }

        public void AddLiveBalanceActiveAndEffectiveCondition(RDBConditionContext conditionContext, string liveBalanceAlias, VRAccountStatus? accountStatus, DateTime? effectiveDate, bool? isEffectiveInFuture)
        {
            var orCondition = conditionContext.Or();
            orCondition.NullCondition(liveBalanceAlias, "AccountID");
            var andCondition = orCondition.And();

            andCondition.ConditionIfColumnNotNull(liveBalanceAlias, "IsDeleted").EqualsCondition(liveBalanceAlias, "IsDeleted", false);
            if (accountStatus.HasValue)
                andCondition.EqualsCondition(liveBalanceAlias, "Status", (int)accountStatus.Value);

            if (effectiveDate.HasValue)
            {
                var andCondition2 = andCondition.And();
                andCondition2.ConditionIfColumnNotNull(liveBalanceAlias, "BED").CompareCondition(liveBalanceAlias, "BED", RDBCompareConditionOperator.LEq, effectiveDate.Value);
                andCondition2.ConditionIfColumnNotNull(liveBalanceAlias, "EED").CompareCondition(liveBalanceAlias, "EED", RDBCompareConditionOperator.G, effectiveDate.Value);
            }

            if(isEffectiveInFuture.HasValue)
            {
                if(isEffectiveInFuture.Value)
                {
                    andCondition.ConditionIfColumnNotNull(liveBalanceAlias, "EED").CompareCondition(liveBalanceAlias, "EED", RDBCompareConditionOperator.GEq, new RDBNowDateTimeExpression());
                }
                else
                {
                    var andCondition2 = andCondition.And();
                    andCondition2.NotNullCondition(liveBalanceAlias, "EED");
                    andCondition2.CompareCondition(liveBalanceAlias, "EED", RDBCompareConditionOperator.LEq, new RDBNowDateTimeExpression());
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
                insertQuery.ColumnValue("ID", lvToUpdate.LiveBalanceId);
                insertQuery.ColumnValue("UpdateValue", lvToUpdate.Value);
            }
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var joinContext = updateQuery.Join("lv");
            joinContext.JoinOnEqualOtherTableColumn(tempTableQuery, "lvToUpdate", "ID", "lv", "ID");

            updateQuery.ColumnValue("CurrentBalance", new RDBArithmeticExpression
                                    {
                                        Operator = RDBArithmeticExpressionOperator.Add,
                                        Expression1 = new RDBColumnExpression { TableAlias = "lv", ColumnName = "CurrentBalance", DontAppendTableAlias = true },
                                        Expression2 = new RDBColumnExpression { TableAlias = "lvToUpdate", ColumnName = "UpdateValue" }
                                    });
        }

        private void AddLiveBalanceToAlertCondition(RDBConditionContext conditionContext, string liveBalanceAlias)
        {
            var andCondition = conditionContext.And();
            andCondition.CompareCondition("CurrentBalance", RDBCompareConditionOperator.LEq, new RDBColumnExpression { TableAlias = liveBalanceAlias, ColumnName = "NextAlertThreshold" });
            andCondition.ConditionIfColumnNotNull("LastExecutedActionThreshold").CompareCondition("NextAlertThreshold", RDBCompareConditionOperator.L, new RDBColumnExpression { TableAlias = liveBalanceAlias, ColumnName = "LastExecutedActionThreshold" });
        }
        private void AddLiveBalanceToClearAlertCondition(RDBConditionContext conditionContext, string liveBalanceAlias)
        {
            conditionContext.CompareCondition("CurrentBalance", RDBCompareConditionOperator.G, new RDBColumnExpression { TableAlias = liveBalanceAlias, ColumnName = "LastExecutedActionThreshold" });
        }

    }
}
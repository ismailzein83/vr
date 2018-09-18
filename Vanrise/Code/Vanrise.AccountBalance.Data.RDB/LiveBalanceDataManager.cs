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
        static string TABLE_NAME = "VR_AccountBalance_LiveBalance";

        const string COL_ID = "ID";
        const string COL_AccountTypeID = "AccountTypeID";
        const string COL_AccountID = "AccountID";
        const string COL_CurrencyID = "CurrencyID";
        const string COL_InitialBalance = "InitialBalance";
        const string COL_CurrentBalance = "CurrentBalance";
        const string COL_NextAlertThreshold = "NextAlertThreshold";
        const string COL_LastExecutedActionThreshold = "LastExecutedActionThreshold";
        const string COL_AlertRuleID = "AlertRuleID";
        const string COL_ActiveAlertsInfo = "ActiveAlertsInfo";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_Status = "Status";
        const string COL_IsDeleted = "IsDeleted";
        const string COL_CreatedTime = "CreatedTime";

        static LiveBalanceDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_AccountTypeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_AccountID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_CurrencyID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_InitialBalance, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6 });
            columns.Add(COL_CurrentBalance, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6 });
            columns.Add(COL_NextAlertThreshold, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6 });
            columns.Add(COL_LastExecutedActionThreshold, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6 });
            columns.Add(COL_AlertRuleID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ActiveAlertsInfo, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_Status, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_IsDeleted, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_AccountBalance",
                DBTableName = "LiveBalance",
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

        private LiveBalance LiveBalanceMapper(IRDBDataReader reader)
        {
            return new LiveBalance
            {
                CurrentBalance = reader.GetDecimal(COL_CurrentBalance),
                AccountId = reader.GetString(COL_AccountID),
                AccountTypeId = reader.GetGuid(COL_AccountTypeID),
                CurrencyId = reader.GetInt(COL_CurrencyID),
                AlertRuleID = reader.GetIntWithNullHandling(COL_AlertRuleID),
                InitialBalance = reader.GetDecimal(COL_InitialBalance),
                NextThreshold = reader.GetNullableDecimal(COL_NextAlertThreshold),
                LastExecutedThreshold = reader.GetNullableDecimal(COL_LastExecutedActionThreshold),
                LiveBalanceActiveAlertsInfo = Serializer.Deserialize(reader.GetString(COL_ActiveAlertsInfo), typeof(VRBalanceActiveAlertInfo)) as VRBalanceActiveAlertInfo,
                BED = reader.GetNullableDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED),
                Status = (VRAccountStatus) reader.GetIntWithNullHandling(COL_Status),
            };
        }

        private Vanrise.AccountBalance.Entities.AccountBalance AccountBalanceMapper(IRDBDataReader reader)
        {
            return new Vanrise.AccountBalance.Entities.AccountBalance
            {
                AccountBalanceId = reader.GetLong(COL_ID),
                AccountId = reader.GetString(COL_AccountID),
                AccountTypeId = reader.GetGuid(COL_AccountTypeID),
                CurrentBalance = reader.GetDecimal(COL_CurrentBalance),
                CurrencyId = reader.GetInt(COL_CurrencyID),
                InitialBalance = reader.GetDecimal(COL_InitialBalance),
                AlertRuleID = reader.GetIntWithNullHandling(COL_AlertRuleID),
                BED = reader.GetNullableDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED),
                Status = (VRAccountStatus)reader.GetIntWithNullHandling(COL_Status)

            };
        }

        private LiveBalanceAccountInfo LiveBalanceAccountInfoMapper(IRDBDataReader reader)
        {
            return new LiveBalanceAccountInfo
            {
                LiveBalanceId = reader.GetLong(COL_ID),
                AccountId = reader.GetString(COL_AccountID),
                CurrencyId = reader.GetInt(COL_CurrencyID),
                BED = reader.GetNullableDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED),
                Status = (VRAccountStatus)reader.GetIntWithNullHandling(COL_Status)
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
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            where.EqualsCondition(COL_AccountID).Value(accountId);
            where.ConditionIfColumnNotNull(COL_IsDeleted).EqualsCondition(COL_IsDeleted).Value(false);
            
            return queryContext.GetItem(LiveBalanceMapper);
        }

        public void GetLiveBalanceAccounts(Guid accountTypeId, Action<LiveBalance> onLiveBalanceReady)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "lb");
            selectQuery.SelectColumns().AllTableColumns("lb");

            var where = selectQuery.Where();
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            where.ConditionIfColumnNotNull(COL_IsDeleted).EqualsCondition(COL_IsDeleted).Value(false);

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
            where.EqualsCondition(COL_AccountTypeID).Value(query.AccountTypeId);
            if (query.AccountsIds != null && query.AccountsIds.Count() > 0)
                where.ListCondition(COL_AccountID, RDBListConditionOperator.IN, query.AccountsIds);
            if (query.Sign != null)
                where.CompareCondition(COL_CurrentBalance, ConvertBalanceCompareSign(query.Sign)).Value(query.Balance);
            AddLiveBalanceActiveAndEffectiveCondition(where, "lb", query.Status, query.EffectiveDate, query.IsEffectiveInFuture);

            var sortContext = selectQuery.Sort();
            sortContext.ByColumn(COL_CurrentBalance, query.OrderBy == "ASC" ? RDBSortDirection.ASC : RDBSortDirection.DESC);

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
            selectQuery.SelectColumns().Columns(COL_ID, COL_AccountID, COL_CurrencyID, COL_BED, COL_EED, COL_Status);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            where.ConditionIfColumnNotNull(COL_IsDeleted).EqualsCondition(COL_IsDeleted).Value(false);

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
            selectQuery.SelectColumns().Columns(COL_ID, COL_CurrencyID);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            where.EqualsCondition(COL_AccountID).Value(accountId);
            where.ConditionIfColumnNotNull(COL_IsDeleted).EqualsCondition(COL_IsDeleted).Value(false);

            queryContext.ExecuteReader((reader) =>
            {
                if (reader.Read())
                {
                    liveBalanceFound = true;
                    liveBalanceInfo.LiveBalanceId = reader.GetLong(COL_ID);
                    liveBalanceInfo.CurrencyId = reader.GetInt(COL_CurrencyID);
                }
            });

            if(!liveBalanceFound)
            {
                queryContext = new RDBQueryContext(GetDataProvider());
                var insertQuery = queryContext.AddInsertQuery();
                insertQuery.IntoTable(TABLE_NAME);
                insertQuery.AddSelectGeneratedId();
                insertQuery.Column(COL_AccountTypeID).Value(accountTypeId);
                insertQuery.Column(COL_AccountID).Value(accountId);
                insertQuery.Column(COL_InitialBalance).Value(initialBalance);
                insertQuery.Column(COL_CurrentBalance).Value(currentBalance);
                insertQuery.Column(COL_CurrencyID).Value(currencyId);
                if (bed.HasValue)
                    insertQuery.Column(COL_BED).Value(bed.Value);
                if(eed.HasValue)
                    insertQuery.Column(COL_EED).Value(eed.Value);
                insertQuery.Column(COL_Status).Value((int)status);
                insertQuery.Column(COL_IsDeleted).Value(isDeleted);
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

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(COL_AccountTypeID, RDBDataType.UniqueIdentifier, true);
            tempTableQuery.AddColumn(COL_AccountID, RDBDataType.Varchar, 50, null, true);
            tempTableQuery.AddColumn(COL_NextAlertThreshold, RDBDataType.Decimal, 20, 6);
            tempTableQuery.AddColumn(COL_AlertRuleID, RDBDataType.Int);

            if (updateEntities != null)
            {
                foreach(var updateEntity in updateEntities)
                {
                    var insertQuery = queryContext.AddInsertQuery();
                    insertQuery.IntoTable(tempTableQuery);
                    insertQuery.Column(COL_AccountTypeID).Value(updateEntity.AccountTypeId);
                    insertQuery.Column(COL_AccountID).Value(updateEntity.AccountId);
                    if (updateEntity.NextAlertThreshold.HasValue)
                        insertQuery.Column(COL_NextAlertThreshold).Value(updateEntity.NextAlertThreshold.Value);
                    if (updateEntity.AlertRuleId.HasValue)
                        insertQuery.Column(COL_AlertRuleID).Value(updateEntity.AlertRuleId.Value);
                }
            }

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            
            var joinContext = updateQuery.Join("lb");
            var joinCondition = joinContext.Join(tempTableQuery, "updateEntities").On();
            joinCondition.EqualsCondition("lb", COL_AccountTypeID, "updateEntities", COL_AccountTypeID);
            joinCondition.EqualsCondition("lb", COL_AccountID, "updateEntities", COL_AccountID);

            updateQuery.Column(COL_NextAlertThreshold).Column("updateEntities", COL_NextAlertThreshold);
            updateQuery.Column(COL_AlertRuleID).Column("updateEntities", COL_AlertRuleID);

            queryContext.ExecuteNonQuery();
        }

        public void UpdateBalanceLastAlertInfos(List<LiveBalanceLastThresholdUpdateEntity> updateEntities)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(COL_AccountTypeID, RDBDataType.UniqueIdentifier, true);
            tempTableQuery.AddColumn(COL_AccountID, RDBDataType.Varchar, 50, null, true);
            tempTableQuery.AddColumn(COL_LastExecutedActionThreshold, RDBDataType.Decimal, 20, 6);
            tempTableQuery.AddColumn(COL_ActiveAlertsInfo, RDBDataType.NVarchar);

            if (updateEntities != null)
            {
                foreach (var updateEntity in updateEntities)
                {
                    var insertQuery = queryContext.AddInsertQuery();
                    insertQuery.IntoTable(tempTableQuery);
                    insertQuery.Column(COL_AccountTypeID).Value(updateEntity.AccountTypeId);
                    insertQuery.Column(COL_AccountID).Value(updateEntity.AccountId);
                    if (updateEntity.LastExecutedActionThreshold.HasValue)
                        insertQuery.Column(COL_LastExecutedActionThreshold).Value(updateEntity.LastExecutedActionThreshold.Value);
                    if (updateEntity.ActiveAlertsInfo != null)
                        insertQuery.Column(COL_ActiveAlertsInfo).Value(Serializer.Serialize(updateEntity.ActiveAlertsInfo, true));
                }
            }

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var joinContext = updateQuery.Join("lb");
            var joinCondition = joinContext.Join(tempTableQuery, "updateEntities").On();
            joinCondition.EqualsCondition("lb", COL_AccountTypeID, "updateEntities", COL_AccountTypeID);
            joinCondition.EqualsCondition("lb", COL_AccountID, "updateEntities", COL_AccountID);

            updateQuery.Column(COL_LastExecutedActionThreshold).Column("updateEntities", COL_LastExecutedActionThreshold);
            updateQuery.Column(COL_ActiveAlertsInfo).Column("updateEntities", COL_ActiveAlertsInfo);

            queryContext.ExecuteNonQuery();
        }

        public void GetLiveBalancesToAlert(Guid accountTypeId, Action<LiveBalance> onLiveBalanceReady)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "lb");
            selectQuery.SelectColumns().AllTableColumns("lb");

            var where = selectQuery.Where();
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            AddLiveBalanceToAlertCondition(where, "lb");
            where.ConditionIfColumnNotNull(COL_IsDeleted).EqualsCondition(COL_IsDeleted).Value(false);

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
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            AddLiveBalanceToClearAlertCondition(where, "lb");
            where.ConditionIfColumnNotNull(COL_IsDeleted).EqualsCondition(COL_IsDeleted).Value(false);

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
            selectQuery.SelectColumns().Column(COL_ID);

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

            updateQuery.Column(COL_Status).Value((int)status);
           updateQuery.Column(COL_IsDeleted).Value(isDeleted);

            var where = updateQuery.Where();
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            where.EqualsCondition(COL_AccountID).Value(accountId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool TryUpdateLiveBalanceEffectiveDate(string accountId, Guid accountTypeId, DateTime? bed, DateTime? eed)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            if (bed.HasValue)
                updateQuery.Column(COL_BED).Value(bed.Value);
            if (eed.HasValue)
                updateQuery.Column(COL_EED).Value(eed.Value);

            var where = updateQuery.Where();
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            where.EqualsCondition(COL_AccountID).Value(accountId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        #endregion

        public void JoinLiveBalance(RDBJoinContext joinContext, string liveBalanceAlias, string originalTableAlias, string originalTableCol_AccountTypeId, string originalTableCol_AccountId)
        {
            var joinStatement = joinContext.Join(TABLE_NAME, liveBalanceAlias);
            joinStatement.JoinType(RDBJoinType.Left);
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(liveBalanceAlias, COL_AccountTypeID, originalTableAlias, originalTableCol_AccountTypeId);
            joinCondition.EqualsCondition(liveBalanceAlias, COL_AccountID, originalTableAlias, originalTableCol_AccountId);                    
        }

        public void AddLiveBalanceActiveAndEffectiveCondition(RDBConditionContext conditionContext, string liveBalanceAlias, VRAccountStatus? accountStatus, DateTime? effectiveDate, bool? isEffectiveInFuture)
        {
            var orCondition = conditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(liveBalanceAlias, COL_AccountID);
            var andCondition = orCondition.ChildConditionGroup();

            andCondition.ConditionIfColumnNotNull(liveBalanceAlias, COL_IsDeleted).EqualsCondition(liveBalanceAlias, COL_IsDeleted).Value(false);
            if (accountStatus.HasValue)
                andCondition.EqualsCondition(liveBalanceAlias, COL_Status).Value((int)accountStatus.Value);

            if (effectiveDate.HasValue)
            {
                andCondition.ConditionIfColumnNotNull(liveBalanceAlias, COL_BED).LessOrEqualCondition(liveBalanceAlias, COL_BED).Value(effectiveDate.Value);
                andCondition.ConditionIfColumnNotNull(liveBalanceAlias, COL_EED).GreaterThanCondition(liveBalanceAlias, COL_EED).Value(effectiveDate.Value);
            }

            if(isEffectiveInFuture.HasValue)
            {
                if(isEffectiveInFuture.Value)
                {
                    andCondition.ConditionIfColumnNotNull(liveBalanceAlias, COL_EED).GreaterOrEqualCondition(liveBalanceAlias, COL_EED).DateNow();
                }
                else
                {
                    andCondition.NotNullCondition(liveBalanceAlias, COL_EED);
                    andCondition.LessOrEqualCondition(liveBalanceAlias, COL_EED).DateNow();
                }
            }
        }

        private void AddQueryUpdateLiveBalances(RDBQueryContext queryContext, IEnumerable<LiveBalanceToUpdate> liveBalancesToUpdate)
        {
            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(COL_ID, RDBDataType.BigInt, true);
            tempTableQuery.AddColumn("UpdateValue", RDBDataType.Decimal, 20, 6);

            foreach (var lvToUpdate in liveBalancesToUpdate)
            {
                var insertQuery = queryContext.AddInsertQuery();
                insertQuery.IntoTable(tempTableQuery);
                insertQuery.Column(COL_ID).Value(lvToUpdate.LiveBalanceId);
                insertQuery.Column("UpdateValue").Value(lvToUpdate.Value);
            }
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var joinContext = updateQuery.Join("lv");
            joinContext.JoinOnEqualOtherTableColumn(tempTableQuery, "lvToUpdate", COL_ID, "lv", COL_ID);

            var currentBalanceExpressionContext = updateQuery.Column(COL_CurrentBalance).ArithmeticExpression(RDBArithmeticExpressionOperator.Add);
            currentBalanceExpressionContext.Expression1().Column("lv", COL_CurrentBalance);
            currentBalanceExpressionContext.Expression2().Column("lvToUpdate", "UpdateValue");
        }

        private void AddLiveBalanceToAlertCondition(RDBConditionContext conditionContext, string liveBalanceAlias)
        {
            var andCondition = conditionContext.ChildConditionGroup();
            andCondition.LessOrEqualCondition(COL_CurrentBalance).Column(liveBalanceAlias, COL_NextAlertThreshold);
            andCondition.ConditionIfColumnNotNull(COL_LastExecutedActionThreshold).LessThanCondition(COL_NextAlertThreshold).Column(liveBalanceAlias, COL_LastExecutedActionThreshold);
        }
        private void AddLiveBalanceToClearAlertCondition(RDBConditionContext conditionContext, string liveBalanceAlias)
        {
            conditionContext.GreaterThanCondition(COL_CurrentBalance).Column(liveBalanceAlias, COL_LastExecutedActionThreshold);
        }

    }
}
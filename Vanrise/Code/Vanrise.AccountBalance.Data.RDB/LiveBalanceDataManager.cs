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
            return new RDBQueryContext(GetDataProvider())
                        .Select()
                        .From(TABLE_NAME, "lb")
                        .Where().And()
                                    .EqualsCondition("AccountTypeID", accountTypeId)
                                    .EqualsCondition("AccountID", accountId)
                                    .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
                                .EndAnd()
                        .SelectColumns().AllTableColumns("lb").EndColumns()
                        .EndSelect()
                        .GetItem(LiveBalanceMapper);
        }

        public void GetLiveBalanceAccounts(Guid accountTypeId, Action<LiveBalance> onLiveBalanceReady)
        {
            new RDBQueryContext(GetDataProvider())
                .Select()
                .From(TABLE_NAME, "lb")
                .Where().And()
                            .EqualsCondition("AccountTypeID", accountTypeId)
                            .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
                        .EndAnd()
                .SelectColumns().AllTableColumns("lb").EndColumns()
                .EndSelect()
                .ExecuteReader(
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
            return new RDBQueryContext(GetDataProvider())
                        .Select()
                        .From(TABLE_NAME, "lb", query.Top)
                        .Where().And()
                                    .EqualsCondition("AccountTypeID", query.AccountTypeId)
                                    .ConditionIf(() => query.AccountsIds != null && query.AccountsIds.Count() > 0, ctx => ctx.ListCondition("AccountID", RDBListConditionOperator.IN, query.AccountsIds))
                                    .ConditionIf(() => query.Sign != null, ctx => ctx.CompareCondition("CurrentBalance", ConvertBalanceCompareSign(query.Sign), query.Balance))
                                    .LiveBalanceActiveAndEffectiveCondition("lb", query.Status, query.EffectiveDate, query.IsEffectiveInFuture)
                                .EndAnd()
                        .SelectColumns().AllTableColumns("lb").EndColumns()
                        .Sort().ByColumn("CurrentBalance", query.OrderBy == "ASC" ? RDBSortDirection.ASC : RDBSortDirection.DESC).EndSort()
                        .EndSelect()
                        .GetItems(AccountBalanceMapper);
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
            new RDBQueryContext(GetDataProvider())
                .StartBatchQuery()
                .AddQueryIf(() => overridenUsageIdsToRollback != null && overridenUsageIdsToRollback.Count() > 0, 
                    ctx => ctx.Update()
                                .FromTable(AccountUsageDataManager.TABLE_NAME)
                                .Where().ListCondition("ID", RDBListConditionOperator.IN, overridenUsageIdsToRollback)
                                .ColumnValue("IsOverridden", new RDBNullExpression()).ColumnValue("OverriddenAmount", new RDBNullExpression())
                              .EndUpdate())
                .AddQueryIf(() => accountUsageIdsToOverride != null && accountUsageIdsToOverride.Count() > 0,
                    ctx => ctx.Update()
                                .FromTable(AccountUsageDataManager.TABLE_NAME)
                                .Where().ListCondition("ID", RDBListConditionOperator.IN, accountUsageIdsToOverride)
                                .ColumnValue("IsOverridden", true).ColumnValue("OverriddenAmount", new RDBColumnExpression { ColumnName = "UsageBalance"})
                              .EndUpdate())
                .AddQueryIf(()=> deletedTransactionIds != null && deletedTransactionIds.Count() > 0,
                    ctx => ctx.StartBatchQuery()
                                .AddQuery().Delete().FromTable(AccountUsageOverrideDataManager.TABLE_NAME).Where().ListCondition("OverriddenByTransactionID", RDBListConditionOperator.IN, deletedTransactionIds).EndDelete()
                                .AddQuery().Update().FromTable(BillingTransactionDataManager.TABLE_NAME).Where().ListCondition("ID", RDBListConditionOperator.IN, deletedTransactionIds).ColumnValue("IsSubtractedFromBalance", true).EndUpdate()
                              .EndBatchQuery())
                .AddQueryIf(() => accountUsageOverrides != null && accountUsageOverrides.Count() > 0,
                    ctx => ctx.StartBatchQuery()
                                .Foreach(accountUsageOverrides, 
                                    (accountUsageOverride, foreachCtx) => foreachCtx.AddQuery().Insert()
                                                                                                .IntoTable(AccountUsageOverrideDataManager.TABLE_NAME)
                                                                                                .ColumnValue("AccountTypeID", accountUsageOverride.AccountTypeId)
                                                                                                .ColumnValue("AccountID", accountUsageOverride.AccountId)
                                                                                                .ColumnValue("TransactionTypeID", accountUsageOverride.TransactionTypeId)
                                                                                                .ColumnValue("PeriodStart", accountUsageOverride.PeriodStart)
                                                                                                .ColumnValue("PeriodEnd", accountUsageOverride.PeriodEnd)
                                                                                                .ColumnValue("OverriddenByTransactionID", accountUsageOverride.OverriddenByTransactionId)
                                                                                               .EndInsert())
                              .EndBatchQuery())
                .AddQueryIf(() => billingTransactionIds != null && billingTransactionIds.Count() > 0,
                    ctx => ctx.Update().FromTable(BillingTransactionDataManager.TABLE_NAME).Where().ListCondition("ID", RDBListConditionOperator.IN, billingTransactionIds).ColumnValue("IsBalanceUpdated", true).EndUpdate())
                .UpdateLiveBalances(liveBalancesToUpdate)
                .EndBatchQuery()
                .ExecuteNonQuery(true);

                                        
            return true;
        }

        public IEnumerable<LiveBalanceAccountInfo> GetLiveBalanceAccountsInfo(Guid accountTypeId)
        {
            return new RDBQueryContext(GetDataProvider())
                       .Select()
                       .From(TABLE_NAME, "lb")
                       .Where().And()
                                   .EqualsCondition("AccountTypeID", accountTypeId)
                                   .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
                               .EndAnd()
                       .SelectColumns().Columns("ID", "AccountID", "CurrencyID", "BED", "EED", "Status").EndColumns()
                       .EndSelect()
                       .GetItems(LiveBalanceAccountInfoMapper);
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
            new RDBQueryContext(GetDataProvider()).Select()
                                                    .From(TABLE_NAME, "lv")
                                                    .Where().And()
                                                                .EqualsCondition("AccountTypeID", accountTypeId)
                                                                .EqualsCondition("AccountID", accountId)
                                                                .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
                                                            .EndAnd()
                                                    .SelectColumns().Columns("ID", "CurrencyID").EndColumns()
                                                    .EndSelect()
                                                    .ExecuteReader((reader) =>
                                                    {
                                                        if(reader.Read())
                                                        {
                                                            liveBalanceFound = true;
                                                            liveBalanceInfo.LiveBalanceId = reader.GetLong("ID");
                                                            liveBalanceInfo.CurrencyId = reader.GetInt("CurrencyID");
                                                        }
                                                    });
            if(!liveBalanceFound)
            {
                liveBalanceInfo.LiveBalanceId = new RDBQueryContext(GetDataProvider()).Insert()
                                                                                        .IntoTable(TABLE_NAME)
                                                                                        .GenerateIdAndAssignToParameter("ID")
                                                                                        .ColumnValue("AccountTypeID", accountTypeId)
                                                                                        .ColumnValue("AccountID", accountId)
                                                                                        .ColumnValue("InitialBalance", initialBalance)
                                                                                        .ColumnValue("CurrentBalance", currentBalance)
                                                                                        .ColumnValue("CurrencyID", currencyId)
                                                                                        .ColumnValueDBNullIfDefaultValue("BED", bed)
                                                                                        .ColumnValueDBNullIfDefaultValue("EED", eed)
                                                                                        .ColumnValue("Status", (int)status)
                                                                                        .ColumnValue("IsDeleted", isDeleted)
                                                                                       .EndInsert()
                                                                                       .ExecuteScalar().LongValue;
                liveBalanceInfo.CurrencyId = currencyId;
            }
            return liveBalanceInfo;

            //return new RDBQueryContext(GetDataProvider())
            //        .StartBatchQuery()
            //        .AddQuery().DeclareParameters().AddParameter("ID", RDBDataType.BigInt).AddParameter("CurrencyIdToReturn", RDBDataType.Int).EndParameterDeclaration()
            //        .AddQuery().Select()
            //                    .From(TABLE_NAME, "lv")
            //                    .Where().And()
            //                                .EqualsCondition("AccountTypeID", accountTypeId)
            //                                .EqualsCondition("AccountID", accountId)
            //                                .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
            //                            .EndAnd()
            //                    .SelectColumns().ColumnToParameter("ID", "ID").ColumnToParameter("CurrencyID", "CurrencyIdToReturn").EndColumns()
            //                    .EndSelect()
            //        .AddQuery().If().IfCondition().NullCondition(new RDBParameterExpression { ParameterName = "ID" })
            //                    .ThenQuery().StartBatchQuery()
            //                                .AddQuery().Insert()
            //                                            .IntoTable(TABLE_NAME)
            //                                            .GenerateIdAndAssignToParameter("ID")
            //                                            .ColumnValue("AccountTypeID", accountTypeId)
            //                                            .ColumnValue("AccountID", accountId)
            //                                            .ColumnValue("InitialBalance", initialBalance)
            //                                            .ColumnValue("CurrentBalance", currentBalance)
            //                                            .ColumnValue("CurrencyID", currencyId)
            //                                            .ColumnValueIfNotDefaultValue(bed, ctx =>  ctx.ColumnValue("BED", bed.Value))
            //                                            .ColumnValueIfNotDefaultValue(eed, ctx => ctx.ColumnValue("EED", eed.Value))
            //                                            .ColumnValue("Status", (int)status)
            //                                            .ColumnValue("IsDeleted", isDeleted)
            //                                           .EndInsert()
            //                                .AddQuery().SetParameterValues().ParamValue("CurrencyIdToReturn", currencyId).EndParameterValues()
            //                                .EndBatchQuery()
            //                    .EndIf()
            //         .AddQuery().Select().FromNoTable().SelectColumns().Parameter("ID", "ID").Parameter("CurrencyID", "CurrencyID").EndColumns().EndSelect()
            //        .EndBatchQuery()
            //        .GetItem(LiveBalanceAccountInfoMapper);
        }

        public bool UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(long balanceUsageQueueId, IEnumerable<LiveBalanceToUpdate> liveBalancesToUpdate, IEnumerable<AccountUsageToUpdate> accountsUsageToUpdate, Guid? correctionProcessId)
        {
            new RDBQueryContext(GetDataProvider())
                .StartBatchQuery()
                    .UpdateLiveBalances(liveBalancesToUpdate)
                    .UpdateAccountUsages(accountsUsageToUpdate, correctionProcessId)
                    .AddQuery().Delete().FromTable(BalanceUsageQueueDataManager.TABLE_NAME).Where().EqualsCondition("ID", balanceUsageQueueId).EndDelete()
                .EndBatchQuery()
                .ExecuteNonQuery(true);
            return true;
        }

        public void UpdateBalanceRuleInfos(List<LiveBalanceNextThresholdUpdateEntity> updateEntities)
        {
            var tempTableColumns = new Dictionary<string, RDBTableColumnDefinition>();
            tempTableColumns.Add("AccountTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            tempTableColumns.Add("AccountID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            tempTableColumns.Add("NextAlertThreshold", new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6 });
            tempTableColumns.Add("AlertRuleID", new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            var tempTableQuery = new RDBTempTableQuery(tempTableColumns);
            new RDBQueryContext(GetDataProvider())
                .StartBatchQuery()
                    .AddQuery().CreateTempTable(tempTableQuery)
                    .Foreach(updateEntities,
                        (updateEntity, ctx) => ctx.AddQuery().Insert().IntoTable(tempTableQuery)
                                                                .ColumnValue("AccountTypeID", updateEntity.AccountTypeId)
                                                                .ColumnValue("AccountID", updateEntity.AccountId)
                                                                .ColumnValueDBNullIfDefaultValue("NextAlertThreshold", updateEntity.NextAlertThreshold)
                                                                .ColumnValueDBNullIfDefaultValue("AlertRuleID", updateEntity.AlertRuleId)
                                                             .EndInsert())
                    .AddQuery()
                        .Update().FromTable(TABLE_NAME)
                            .Join("lb")
                            .Join(RDBJoinType.Inner, tempTableQuery, "updateEntities").And().EqualsCondition("lb", "AccountTypeID", "updateEntities", "AccountTypeID").EqualsCondition("lb", "AccountID", "updateEntities", "AccountID").EndAnd()
                            .EndJoin()
                            .ColumnValue("NextAlertThreshold", new RDBColumnExpression { TableAlias = "updateEntities", ColumnName = "NextAlertThreshold" })
                            .ColumnValue("AlertRuleID", new RDBColumnExpression { TableAlias = "updateEntities", ColumnName = "AlertRuleID" })
                        .EndUpdate()
                .EndBatchQuery()
                .ExecuteNonQuery();
        }

        public void UpdateBalanceLastAlertInfos(List<LiveBalanceLastThresholdUpdateEntity> updateEntities)
        {
            var tempTableColumns = new Dictionary<string, RDBTableColumnDefinition>();
            tempTableColumns.Add("AccountTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            tempTableColumns.Add("AccountID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            tempTableColumns.Add("LastExecutedActionThreshold", new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6 });
            tempTableColumns.Add("ActiveAlertsInfo", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            var tempTableQuery = new RDBTempTableQuery(tempTableColumns);
            new RDBQueryContext(GetDataProvider())
               .StartBatchQuery()
                   .AddQuery().CreateTempTable(tempTableQuery)
                   .Foreach(updateEntities,
                       (updateEntity, ctx) => ctx.AddQuery().Insert().IntoTable(tempTableQuery)
                                                               .ColumnValue("AccountTypeID", updateEntity.AccountTypeId)
                                                               .ColumnValue("AccountID", updateEntity.AccountId)
                                                               .ColumnValueDBNullIfDefaultValue("LastExecutedActionThreshold", updateEntity.LastExecutedActionThreshold)
                                                               .ColumnValueIf(() => updateEntity.ActiveAlertsInfo != null, insertCtx => insertCtx.ColumnValue("ActiveAlertsInfo", Serializer.Serialize(updateEntity.ActiveAlertsInfo, true)))
                                                            .EndInsert())
                   .AddQuery()
                       .Update().FromTable(TABLE_NAME)
                           .Join("lb")
                           .Join(RDBJoinType.Inner, tempTableQuery, "updateEntities").And().EqualsCondition("lb", "AccountTypeID", "updateEntities", "AccountTypeID").EqualsCondition("lb", "AccountID", "updateEntities", "AccountID").EndAnd()
                           .EndJoin()
                           .ColumnValue("LastExecutedActionThreshold", new RDBColumnExpression { TableAlias = "updateEntities", ColumnName = "LastExecutedActionThreshold" })
                           .ColumnValue("ActiveAlertsInfo", new RDBColumnExpression { TableAlias = "updateEntities", ColumnName = "ActiveAlertsInfo" })
                       .EndUpdate()
               .EndBatchQuery()
               .ExecuteNonQuery();
        }

        public void GetLiveBalancesToAlert(Guid accountTypeId, Action<LiveBalance> onLiveBalanceReady)
        {
            new RDBQueryContext(GetDataProvider())
                    .Select()
                        .From(TABLE_NAME, "lb")
                        .Where().And()
                                    .EqualsCondition("AccountTypeID", accountTypeId)
                                    .LiveBalanceToAlertCondition("lb")
                                    .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
                                .EndAnd()
                        .SelectColumns().AllTableColumns("lb").EndColumns()
                    .EndSelect()
                    .ExecuteReader(reader =>
                    {
                        while(reader.Read())
                        {
                            onLiveBalanceReady(LiveBalanceMapper(reader));
                        }
                    });
        }

        public void GetLiveBalancesToClearAlert(Guid accountTypeId, Action<LiveBalance> onLiveBalanceReady)
        {
            new RDBQueryContext(GetDataProvider())
                   .Select()
                       .From(TABLE_NAME, "lb")
                       .Where().And()
                                   .EqualsCondition("AccountTypeID", accountTypeId)
                                   .LiveBalanceToClearAlertCondition("lb")
                                   .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
                               .EndAnd()
                       .SelectColumns().AllTableColumns("lb").EndColumns()
                   .EndSelect()
                   .ExecuteReader(reader =>
                   {
                       while (reader.Read())
                       {
                           onLiveBalanceReady(LiveBalanceMapper(reader));
                       }
                   });
        }

        public bool HasLiveBalancesUpdateData(Guid accountTypeId)
        {
            return new RDBQueryContext(GetDataProvider())
                        .Select()
                        .From(TABLE_NAME, "lb", 1)
                            .Where()
                                .And()
                                    .EqualsCondition("AccountTypeID", accountTypeId)
                                    .Or()
                                        .And().LiveBalanceToAlertCondition("lb").EndAnd()
                                        .And().LiveBalanceToClearAlertCondition("lb").EndAnd()
                                    .EndOr()
                                .EndAnd()
                            .SelectColumns().Column("ID").EndColumns()
                        .EndSelect()
                        .ExecuteScalar().NullableLongValue.HasValue;

            //return new RDBQueryContext(GetDataProvider())
            //        .If().IfCondition()
            //                .ExistsCondition()
            //                .From(TABLE_NAME, "lb", 1)
            //                .Where()
            //                    .And()
            //                        .EqualsCondition("AccountTypeID", accountTypeId)
            //                        .Or()
            //                            .And().LiveBalanceToAlertCondition("lb").EndAnd()
            //                            .And().LiveBalanceToClearAlertCondition("lb").EndAnd()
            //                        .EndOr()
            //                    .EndAnd()
            //                .SelectColumns().Column("ID").EndColumns()
            //                .EndSelect()
            //        .ThenQuery().Select().FromNoTable().SelectColumns().FixedValue(true, "rslt").EndColumns().EndSelect()
            //        .ElseQuery().Select().FromNoTable().SelectColumns().FixedValue(false, "rslt").EndColumns().EndSelect()
            //        .EndIf()
            //        .ExecuteScalar().BooleanValue;
        }

        public bool CheckIfAccountHasTransactions(Guid accountTypeId, string accountId)
        {
            throw new NotImplementedException();
        }

        public bool TryUpdateLiveBalanceStatus(string accountId, Guid accountTypeId, VRAccountStatus status, bool isDeleted)
        {
            return new RDBQueryContext(GetDataProvider())
                        .Update()
                        .FromTable(TABLE_NAME)
                        .Where().And()
                                    .EqualsCondition("AccountTypeID", accountTypeId)
                                    .EqualsCondition("AccountID", accountId)
                                .EndAnd()
                        .ColumnValue("Status", (int)status)
                        .ColumnValue("IsDeleted", isDeleted)
                        .EndUpdate()
                        .ExecuteNonQuery() > 0;
        }

        public bool TryUpdateLiveBalanceEffectiveDate(string accountId, Guid accountTypeId, DateTime? bed, DateTime? eed)
        {
            return new RDBQueryContext(GetDataProvider())
                        .Update()
                        .FromTable(TABLE_NAME)
                        .Where().And()
                                    .EqualsCondition("AccountTypeID", accountTypeId)
                                    .EqualsCondition("AccountID", accountId)
                                .EndAnd()
                        .ColumnValueDBNullIfDefaultValue("BED", bed)
                        .ColumnValueDBNullIfDefaultValue("EED", eed)
                        .EndUpdate()
                        .ExecuteNonQuery() > 0;
        }

        #endregion
    }
}
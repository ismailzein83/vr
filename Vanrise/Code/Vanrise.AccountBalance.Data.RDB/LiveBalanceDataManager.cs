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
        //private Vanrise.AccountBalance.Entities.AccountBalance AccountBalanceMapper(IRDBDataReader reader)
        //{
        //    return new Vanrise.AccountBalance.Entities.AccountBalance
        //    {
        //        AccountBalanceId = (long)reader["ID"],
        //        AccountId = reader["AccountId"] as String,
        //        AccountTypeId = GetReaderValue<Guid>(reader, "AccountTypeID"),
        //        CurrentBalance = GetReaderValue<Decimal>(reader, "CurrentBalance"),
        //        CurrencyId = GetReaderValue<int>(reader, "CurrencyID"),
        //        InitialBalance = GetReaderValue<Decimal>(reader, "InitialBalance"),
        //        AlertRuleID = GetReaderValue<int>(reader, "AlertRuleID"),
        //        BED = GetReaderValue<DateTime?>(reader, "BED"),
        //        EED = GetReaderValue<DateTime?>(reader, "EED"),
        //        Status = GetReaderValue<VRAccountStatus>(reader, "Status"),

        //    };
        //}

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

        public LiveBalance GetLiveBalance(Guid accountTypeId, string accountId)
        {
            throw new NotImplementedException();
        }

        public void GetLiveBalanceAccounts(Guid accountTypeId, Action<LiveBalance> onLiveBalanceReady)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Entities.AccountBalance> GetFilteredAccountBalances(AccountBalanceQuery query)
        {
            throw new NotImplementedException();
        }

        public bool UpdateLiveBalancesFromBillingTransactions(IEnumerable<LiveBalanceToUpdate> liveBalancesToUpdate, IEnumerable<long> billingTransactionIds, IEnumerable<long> accountUsageIdsToOverride, IEnumerable<AccountUsageOverride> accountUsageOverrides, IEnumerable<long> overridenUsageIdsToRollback, IEnumerable<long> deletedTransactionIds)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LiveBalanceAccountInfo> GetLiveBalanceAccountsInfo(Guid accountTypeId)
        {
            throw new NotImplementedException();
        }

        public LiveBalanceAccountInfo TryAddLiveBalanceAndGet(string accountId, Guid accountTypeId, decimal initialBalance, int currencyId, decimal currentBalance, DateTime? bed, DateTime? eed, VRAccountStatus status, bool isDeleted)
        {
            return new RDBQueryContext(GetDataProvider())
                    .StartBatchQuery()
                    .AddQuery().DeclareParameters().AddParameter("ID", RDBDataType.BigInt).AddParameter("CurrencyIdToReturn", RDBDataType.Int).EndParameterDeclaration()
                    .AddQuery().Select()
                                .From(TABLE_NAME, "lv")
                                .Where().And()
                                            .EqualsCondition("AccountTypeID", accountTypeId)
                                            .EqualsCondition("AccountID", accountId)
                                            .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
                                        .EndAnd()
                                .SelectColumns().ColumnToParameter("ID", "ID").ColumnToParameter("CurrencyID", "CurrencyIdToReturn").EndColumns()
                                .EndSelect()
                    .AddQuery().If().IfCondition().NullCondition(new RDBParameterExpression { ParameterName = "ID" })
                                .ThenQuery().StartBatchQuery()
                                            .AddQuery().Insert()
                                                        .IntoTable(TABLE_NAME)
                                                        .GenerateIdAndAssignToParameter("ID")
                                                        .ColumnValue("AccountTypeID", accountTypeId)
                                                        .ColumnValue("AccountID", accountId)
                                                        .ColumnValue("InitialBalance", initialBalance)
                                                        .ColumnValue("CurrentBalance", currentBalance)
                                                        .ColumnValue("CurrencyID", currencyId)
                                                        .ColumnValueIfNotDefaultValue(bed, ctx =>  ctx.ColumnValue("BED", bed.Value))
                                                        .ColumnValueIfNotDefaultValue(eed, ctx => ctx.ColumnValue("EED", eed.Value))
                                                        .ColumnValue("Status", (int)status)
                                                        .ColumnValue("IsDeleted", isDeleted)
                                                       .EndInsert()
                                            .AddQuery().SetParameterValues().ParamValue("CurrencyIdToReturn", currencyId).EndParameterValues()
                                            .EndBatchQuery()
                                .EndIf()
                     .AddQuery().Select().FromNoTable().SelectColumns().Parameter("ID", "ID").Parameter("AccountID", "AccountID").EndColumns().EndSelect()
                    .EndBatchQuery()
                    .GetItem(LiveBalanceAccountInfoMapper);
        }

        public bool UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(long balanceUsageQueueId, IEnumerable<LiveBalanceToUpdate> liveBalnacesToUpdate, IEnumerable<AccountUsageToUpdate> accountsUsageToUpdate, Guid? correctionProcessId)
        {
            throw new NotImplementedException();
        }

        public void UpdateBalanceRuleInfos(List<LiveBalanceNextThresholdUpdateEntity> updateEntities)
        {
            throw new NotImplementedException();
        }

        public void UpdateBalanceLastAlertInfos(List<LiveBalanceLastThresholdUpdateEntity> updateEntities)
        {
            throw new NotImplementedException();
        }

        public void GetLiveBalancesToAlert(Guid accountTypeId, Action<LiveBalance> onLiveBalanceReady)
        {
            throw new NotImplementedException();
        }

        public void GetLiveBalancesToClearAlert(Guid accountTypeId, Action<LiveBalance> onLiveBalanceReady)
        {
            throw new NotImplementedException();
        }

        public bool HasLiveBalancesUpdateData(Guid accountTypeId)
        {
            throw new NotImplementedException();
        }

        public bool CheckIfAccountHasTransactions(Guid accountTypeId, string accountId)
        {
            throw new NotImplementedException();
        }

        public bool TryUpdateLiveBalanceStatus(string accountId, Guid accountTypeId, VRAccountStatus status, bool isDeleted)
        {
            throw new NotImplementedException();
        }

        public bool TryUpdateLiveBalanceEffectiveDate(string accountId, Guid accountTypeId, DateTime? bed, DateTime? eed)
        {
            throw new NotImplementedException();
        }
    }
}
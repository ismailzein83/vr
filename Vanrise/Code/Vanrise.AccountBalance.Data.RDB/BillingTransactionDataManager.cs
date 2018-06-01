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
    public class BillingTransactionDataManager : IBillingTransactionDataManager
    {
        public static string TABLE_NAME = "VR_AccountBalance_BillingTransaction";
        static BillingTransactionDataManager()
        {
            var columns = new Dictionary<string,RDBTableColumnDefinition>();
            columns.Add("ID", new RDBTableColumnDefinition{ DataType = RDBDataType.BigInt});
            columns.Add("AccountTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier});
            columns.Add("AccountID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add("TransactionTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("Amount", new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6 });
            columns.Add("CurrencyId", new RDBTableColumnDefinition {  DataType = RDBDataType.Int });
            columns.Add("TransactionTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("Reference", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add("Notes", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 1000 });
            columns.Add("CreatedByInvoiceID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("IsBalanceUpdated", new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add("Settings", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add("IsDeleted", new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add("IsSubtractedFromBalance", new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add("SourceID", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add("CreatedTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_AccountBalance",
                DBTableName = "BillingTransaction",
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

        private BillingTransaction BillingTransactionMapper(IRDBDataReader reader)
        {
            string settingsAsString = reader.GetString("Settings");

            return new BillingTransaction
            {
                AccountBillingTransactionId = reader.GetLong("ID"),
                AccountId = reader.GetString("AccountID"),
                AccountTypeId = reader.GetGuid("AccountTypeId"),
                Amount = reader.GetDecimal("Amount"),
                CurrencyId = reader.GetInt("CurrencyId"),
                Notes = reader.GetString("Notes"),
                TransactionTime = reader.GetDateTime("TransactionTime"),
                TransactionTypeId = reader.GetGuid("TransactionTypeID"),
                Reference = reader.GetString("Reference"),
                IsBalanceUpdated = reader.GetBooleanWithNullHandling("IsBalanceUpdated"),
                SourceId = reader.GetString("SourceId"),
                Settings = (settingsAsString != null) ? Vanrise.Common.Serializer.Deserialize<BillingTransactionSettings>(settingsAsString) : null,
                IsDeleted = reader.GetBooleanWithNullHandling("IsDeleted"),
                IsSubtractedFromBalance = reader.GetBooleanWithNullHandling("IsSubtractedFromBalance")
            };
        }


        private BillingTransactionMetaData BillingTransactionMetaDataMapper(IRDBDataReader reader)
        {
            return new BillingTransactionMetaData
            {
                AccountId = reader.GetString("AccountID"),
                Amount = reader.GetDecimal("Amount"),
                CurrencyId = reader.GetInt("CurrencyId"),
                TransactionTime = reader.GetDateTime("TransactionTime"),
                TransactionTypeId = reader.GetGuid("TransactionTypeID"),
            };
        }

        #endregion

        #region IBillingTransactionDataManager

        public IEnumerable<BillingTransaction> GetFilteredBillingTransactions(BillingTransactionQuery query)
        {
            return new RDBQueryContext(GetDataProvider())
                   .Select()
                   .From(TABLE_NAME, "bt")
                   .Join()
                   .JoinLiveBalance("liveBalance", "bt")
                   .EndJoin()
                   .Where().And()
                                .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
                                .ConditionIf(() => query.AccountsIds != null && query.AccountsIds.Count() > 0, ctx => ctx.ListCondition("AccountID", RDBListConditionOperator.IN, query.AccountsIds))
                                .ConditionIf(() => query.TransactionTypeIds != null && query.TransactionTypeIds.Count() > 0, ctx => ctx.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, query.TransactionTypeIds))
                                .CompareCondition("TransactionTime", RDBCompareConditionOperator.GEq, query.FromTime)
                                .ConditionIf(() => query.ToTime.HasValue, ctx => ctx.CompareCondition("TransactionTime", RDBCompareConditionOperator.LEq, query.ToTime.Value))
                                .EqualsCondition("AccountTypeID", query.AccountTypeId)
                                .LiveBalanceActiveAndEffectiveCondition("liveBalance", query.Status, query.EffectiveDate, query.IsEffectiveInFuture)
                           .EndAnd()
                   .SelectColumns().AllTableColumns("bt").EndColumns()
                   .EndSelect()                   
                   .GetItems(BillingTransactionMapper);  
        }

        public bool Insert(BillingTransaction billingTransaction, out long billingTransactionId)
        {
            return Insert(billingTransaction, null, out billingTransactionId);
        }

        public bool Insert(BillingTransaction billingTransaction, long? invoiceId, out long billingTransactionId)
        {
            billingTransactionId = new RDBQueryContext(GetDataProvider())
                .Insert()
                .IntoTable(TABLE_NAME)
                .GenerateIdAndAssignToParameter("BillingTransactionId")
                .ColumnValue("AccountID", billingTransaction.AccountId)
                .ColumnValue("AccountTypeID", billingTransaction.AccountTypeId)
                .ColumnValue("Amount", billingTransaction.Amount)
                .ColumnValue("CurrencyId", billingTransaction.CurrencyId)
                .ColumnValue("TransactionTypeID", billingTransaction.TransactionTypeId)
                .ColumnValue("TransactionTime", billingTransaction.TransactionTime)
                .ColumnValue("Notes", billingTransaction.Notes)
                .ColumnValue("Reference", billingTransaction.Reference)
                .ColumnValue("SourceID", billingTransaction.SourceId)
                .ColumnValueIf(() => billingTransaction.Settings != null, ctx => ctx.ColumnValue("Settings", Vanrise.Common.Serializer.Serialize(billingTransaction.Settings)))
                .ColumnValueIf(() => invoiceId.HasValue, ctx => ctx.ColumnValue("CreatedByInvoiceID", invoiceId.Value))
                .EndInsert()
                .ExecuteScalar()
                .LongValue;
            return true;
        }

        public void GetBillingTransactionsByBalanceUpdated(Guid accountTypeId, Action<BillingTransaction> onBillingTransactionReady)
        {
            new RDBQueryContext(GetDataProvider())
                   .Select()
                   .From(TABLE_NAME, "bt")
                   .Where().And()
                                .EqualsCondition("AccountTypeID", accountTypeId)
                                .Or()
                                    .And()
                                        .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
                                        .ConditionIfColumnNotNull("IsBalanceUpdated").EqualsCondition("IsBalanceUpdated", false)
                                    .EndAnd()
                                    .And()
                                        .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", true)
                                        .ConditionIfColumnNotNull("IsBalanceUpdated").EqualsCondition("IsBalanceUpdated", true)
                                        .ConditionIfColumnNotNull("IsSubtractedFromBalance").EqualsCondition("IsSubtractedFromBalance", false)
                                    .EndAnd()
                                .EndOr()
                           .EndAnd()
                    .SelectColumns().AllTableColumns("bt").EndColumns()
                    .EndSelect()
                    .ExecuteReader((reader) =>
                    {
                        while (reader.Read())
                        {
                            onBillingTransactionReady(BillingTransactionMapper(reader));
                        }
                    });
        }

        public IEnumerable<BillingTransaction> GetBillingTransactionsForSynchronizerProcess(List<Guid> billingTransactionTypeIds, Guid accountTypeId)
        {
            return new RDBQueryContext(GetDataProvider())
                   .Select()
                   .From(TABLE_NAME, "bt")
                   .Where().And()
                                .EqualsCondition("AccountTypeID", accountTypeId)
                                .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
                                .NotNullCondition("SourceID")
                                .ConditionIf(() => billingTransactionTypeIds != null && billingTransactionTypeIds.Count() > 0, ctx => ctx.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, billingTransactionTypeIds))
                           .EndAnd()
                    .SelectColumns().AllTableColumns("bt").EndColumns()
                    .EndSelect()
                    .GetItems(BillingTransactionMapper);
        }

        public List<BillingTransactionMetaData> GetBillingTransactionsByAccountIds(Guid accountTypeId, List<Guid> transactionTypeIds, List<string> accountIds)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BillingTransactionMetaData> GetBillingTransactionsByTransactionTypes(Guid accountTypeId, List<BillingTransactionByTime> billingTransactionsByTime, List<Guid> transactionTypeIds)
        {
            var btAccountTimesTempTableColumns = new Dictionary<string, RDBTableColumnDefinition>();
            btAccountTimesTempTableColumns.Add("AccountID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            btAccountTimesTempTableColumns.Add("TransactionTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            var btAccountTimesTempTableQuery = new RDBTempTableQuery(btAccountTimesTempTableColumns);

            return new RDBQueryContext(GetDataProvider())
                .StartBatchQuery()
                .AddQuery().CreateTempTable(btAccountTimesTempTableQuery)
                .Foreach(billingTransactionsByTime, (item, batchQuery) =>
                    {
                        batchQuery.AddQuery().Insert().IntoTable(btAccountTimesTempTableQuery).ColumnValue("AccountID", item.AccountId).ColumnValue("TransactionTime", item.TransactionTime);
                    })
                .AddQuery()
                    .Select()
                    .From(TABLE_NAME, "bt")
                    .Join()
                    .Join(RDBJoinType.Inner, btAccountTimesTempTableQuery, "btAccountTimes")
                        .And()
                            .EqualsCondition("bt", "AccountID", new RDBColumnExpression { TableAlias = "btAccountTimes", ColumnName = "AccountID" })
                            .CompareCondition("bt", "TransactionTime", RDBCompareConditionOperator.G, new RDBColumnExpression { TableAlias = "btAccountTimes", ColumnName = "TransactionTime" })
                        .EndAnd()
                    .EndJoin()
                    .Where().And()
                                .EqualsCondition("AccountTypeID", accountTypeId)
                                .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
                                .ConditionIf(() => transactionTypeIds != null && transactionTypeIds.Count() > 0, ctx => ctx.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, transactionTypeIds))
                            .EndAnd()
                    .SelectColumns().Columns("AccountID", "Amount", "CurrencyId", "TransactionTime", "TransactionTypeID").EndColumns()
                    .EndSelect()
                .EndBatchQuery()
                .GetItems(BillingTransactionMetaDataMapper);
        }

        public IEnumerable<BillingTransaction> GetBillingTransactionsByAccountId(Guid accountTypeId, string accountId, Vanrise.Entities.VRAccountStatus? status, DateTime? effectiveDate, bool? isEffectiveInFuture)
        {
    //        SELECT	bt.[ID],bt.AccountID,bt.AccountTypeID, TransactionTypeID,Amount,bt.CurrencyId,TransactionTime,Notes,Reference,IsBalanceUpdated,ClosingPeriodId, SourceID, Settings, bt.IsDeleted, IsSubtractedFromBalance
    //FROM VR_AccountBalance.BillingTransaction bt with(nolock)
    //Inner Join [VR_AccountBalance].LiveBalance vrlb
    //    on vrlb.AccountTypeID = bt.AccountTypeID and 
    //       vrlb.AccountID = bt.AccountID and 
    //       ISNULL(vrlb.IsDeleted, 0) = 0 and
    //       (@Status IS NULL OR vrlb.[Status] = @Status) AND
    //       (@EffectiveDate IS NULL OR ((vrlb.BED <= @EffectiveDate OR vrlb.BED IS NULL) AND (vrlb.EED > @EffectiveDate OR vrlb.EED IS NULL))) AND
    //       (@IsEffectiveInFuture IS NUll OR (@IsEffectiveInFuture = 1 and (vrlb.EED IS NULL or vrlb.EED >=  GETDATE()))  OR  (@IsEffectiveInFuture = 0 and  vrlb.EED <=  GETDATE()))

    //where isnull(bt.IsDeleted, 0) = 0
    //    and bt.AccountTypeID = @AccountTypeID 
    //    AND bt.AccountId= @AccountId
            return new RDBQueryContext(GetDataProvider())
                        .Select()
                        .From(TABLE_NAME, "bt")
                        .Join().JoinLiveBalance("liveBalance", "bt")
                        .EndJoin()
                        .Where().And()
                                    .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
                                    .EqualsCondition("AccountTypeID", accountTypeId)
                                    .EqualsCondition("AccountID", accountId)
                                    .LiveBalanceActiveAndEffectiveCondition("liveBalance", status, effectiveDate, isEffectiveInFuture)
                                .EndAnd()
                        .SelectColumns().AllTableColumns("bt").EndColumns()
                        .EndSelect()
                        .GetItems(BillingTransactionMapper);

        }

        public IEnumerable<BillingTransaction> GetBillingTransactions(List<Guid> accountTypeIds, List<string> accountIds, List<Guid> transactionTypeIds, DateTime fromDate, DateTime? toDate)
        {
            throw new NotImplementedException();
        }

        public BillingTransaction GetLastBillingTransaction(Guid accountTypeId, string accountId)
        {
            return new RDBQueryContext(GetDataProvider())
                   .Select()
                   .From(TABLE_NAME, "bt", 1)
                   .Where().And()
                                .EqualsCondition("AccountTypeID", accountTypeId)
                                .EqualsCondition("AccountID", accountId)
                                .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
                           .EndAnd()
                   .SelectColumns().AllTableColumns("bt").EndColumns()
                   .Sort().ByColumn("TransactionTime", RDBSortDirection.DESC).EndSort()
                   .EndSelect()
                   .GetItem(BillingTransactionMapper);
        }

        public bool HasBillingTransactionData(Guid accountTypeId)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

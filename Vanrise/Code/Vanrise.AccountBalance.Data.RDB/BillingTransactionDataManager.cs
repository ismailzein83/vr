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
            LiveBalanceDataManager liveBalanceDataManager = new LiveBalanceDataManager();
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "bt");
            selectQuery.SelectColumns().AllTableColumns("bt");

            var joinContext = selectQuery.Join();
            liveBalanceDataManager.JoinLiveBalance(joinContext, "liveBalance", "bt");
            
            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false);
            if (query.AccountsIds != null && query.AccountsIds.Count() > 0)
                whereAndCondition.ListCondition("AccountID", RDBListConditionOperator.IN, query.AccountsIds);
            if (query.TransactionTypeIds != null && query.TransactionTypeIds.Count() > 0)
                whereAndCondition.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, query.TransactionTypeIds);
            whereAndCondition.CompareCondition("TransactionTime", RDBCompareConditionOperator.GEq, query.FromTime);
            if (query.ToTime.HasValue)
                whereAndCondition.CompareCondition("TransactionTime", RDBCompareConditionOperator.LEq, query.ToTime.Value);
            whereAndCondition.EqualsCondition("AccountTypeID", query.AccountTypeId);
            liveBalanceDataManager.AddLiveBalanceActiveAndEffectiveCondition(whereAndCondition, "liveBalance", query.Status, query.EffectiveDate, query.IsEffectiveInFuture);
            
            return queryContext.GetItems(BillingTransactionMapper);
        }

        public bool Insert(BillingTransaction billingTransaction, out long billingTransactionId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            AddQueryInsertBillingTransaction(queryContext, billingTransaction, null, true);
            billingTransactionId = queryContext.ExecuteScalar().LongValue;
            return true;
        }

        public void GetBillingTransactionsByBalanceUpdated(Guid accountTypeId, Action<BillingTransaction> onBillingTransactionReady)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "bt");
            selectQuery.SelectColumns().AllTableColumns("bt");

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            AddBillingTransactionsToUpdateBalanceCondition(whereAndCondition);

            queryContext.ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    onBillingTransactionReady(BillingTransactionMapper(reader));
                }
            });
        }

        public IEnumerable<BillingTransaction> GetBillingTransactionsForSynchronizerProcess(List<Guid> billingTransactionTypeIds, Guid accountTypeId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "bt");
            selectQuery.SelectColumns().AllTableColumns("bt");

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            whereAndCondition.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false);
            whereAndCondition.NotNullCondition("SourceID");
            if (billingTransactionTypeIds != null && billingTransactionTypeIds.Count() > 0)
                whereAndCondition.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, billingTransactionTypeIds);

            return queryContext.GetItems(BillingTransactionMapper);
        }

        public List<BillingTransactionMetaData> GetBillingTransactionsByAccountIds(Guid accountTypeId, List<Guid> transactionTypeIds, List<string> accountIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "bt");
            selectQuery.SelectColumns().Columns("AccountID", "Amount", "CurrencyId", "TransactionTime", "TransactionTypeID");

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            whereAndCondition.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false);
            if (accountIds != null && accountIds.Count() > 0)
                whereAndCondition.ListCondition("AccountID", RDBListConditionOperator.IN, accountIds);
            if (transactionTypeIds != null && transactionTypeIds.Count() > 0)
                whereAndCondition.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, transactionTypeIds);
            
            return queryContext.GetItems(BillingTransactionMetaDataMapper);
        }

        public IEnumerable<BillingTransactionMetaData> GetBillingTransactionsByTransactionTypes(Guid accountTypeId, List<BillingTransactionByTime> billingTransactionsByTime, List<Guid> transactionTypeIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var btAccountTimesTempTableColumns = new Dictionary<string, RDBTableColumnDefinition>();
            btAccountTimesTempTableColumns.Add("AccountID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            btAccountTimesTempTableColumns.Add("TransactionTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            var btAccountTimesTempTableQuery = queryContext.CreateTempTable();
            btAccountTimesTempTableQuery.AddColumns(btAccountTimesTempTableColumns);
            
            foreach (var item in billingTransactionsByTime)
            {
                var insertQuery = queryContext.AddInsertQuery();
                insertQuery.IntoTable(btAccountTimesTempTableQuery);
                insertQuery.ColumnValue("AccountID", item.AccountId);
                insertQuery.ColumnValue("TransactionTime", item.TransactionTime);
            }

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "bt");
            selectQuery.SelectColumns().Columns("AccountID", "Amount", "CurrencyId", "TransactionTime", "TransactionTypeID");

            var joinContext = selectQuery.Join();
            var joinCondition = joinContext.Join(RDBJoinType.Inner, btAccountTimesTempTableQuery, "btAccountTimes");
            var joinAndCondition = joinCondition.And();
            joinAndCondition.EqualsCondition("bt", "AccountID", new RDBColumnExpression { TableAlias = "btAccountTimes", ColumnName = "AccountID" });
            joinAndCondition.CompareCondition("bt", "TransactionTime", RDBCompareConditionOperator.G, new RDBColumnExpression { TableAlias = "btAccountTimes", ColumnName = "TransactionTime" });

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            whereAndCondition.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false);
            if (transactionTypeIds != null && transactionTypeIds.Count() > 0)
                whereAndCondition.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, transactionTypeIds);

            return queryContext.GetItems(BillingTransactionMetaDataMapper);
        }

        public IEnumerable<BillingTransaction> GetBillingTransactionsByAccountId(Guid accountTypeId, string accountId, Vanrise.Entities.VRAccountStatus? status, DateTime? effectiveDate, bool? isEffectiveInFuture)
        {
            var liveBalanceDataManager = new LiveBalanceDataManager();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "bt");
            selectQuery.SelectColumns().AllTableColumns("bt");

            var joinContext = selectQuery.Join();
            liveBalanceDataManager.JoinLiveBalance(joinContext, "liveBalance", "bt");

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false);
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            whereAndCondition.EqualsCondition("AccountID", accountId);
            liveBalanceDataManager.AddLiveBalanceActiveAndEffectiveCondition(whereAndCondition, "liveBalance", status, effectiveDate, isEffectiveInFuture);

            return queryContext.GetItems(BillingTransactionMapper);

        }

        public IEnumerable<BillingTransaction> GetBillingTransactions(List<Guid> accountTypeIds, List<string> accountIds, List<Guid> transactionTypeIds, DateTime fromDate, DateTime? toDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "bt");
            selectQuery.SelectColumns().AllTableColumns("bt");

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false);
            if (accountTypeIds != null && accountTypeIds.Count() > 0)
                whereAndCondition.ListCondition("AccountTypeID", RDBListConditionOperator.IN, accountTypeIds);
            if (accountIds != null && accountIds.Count() > 0)
                whereAndCondition.ListCondition("AccountID", RDBListConditionOperator.IN, accountIds);
            if (transactionTypeIds != null && transactionTypeIds.Count() > 0)
                whereAndCondition.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, transactionTypeIds);
            whereAndCondition.CompareCondition("TransactionTime", RDBCompareConditionOperator.GEq, fromDate);
            if (!toDate.HasValue)
                whereAndCondition.CompareCondition("TransactionTime", RDBCompareConditionOperator.LEq, toDate.Value);

            return queryContext.GetItems(BillingTransactionMapper);
        }

        public BillingTransaction GetLastBillingTransaction(Guid accountTypeId, string accountId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "bt", 1);
            selectQuery.SelectColumns().AllTableColumns("bt");

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            whereAndCondition.EqualsCondition("AccountID", accountId);
            whereAndCondition.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false);

            var sortContext = selectQuery.Sort();
            sortContext.ByColumn("TransactionTime", RDBSortDirection.DESC);

            return queryContext.GetItem(BillingTransactionMapper);
        }

        public bool HasBillingTransactionData(Guid accountTypeId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "bt", 1);
            selectQuery.SelectColumns().Column("ID");

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            AddBillingTransactionsToUpdateBalanceCondition(whereAndCondition);

            return queryContext.ExecuteScalar().NullableLongValue.HasValue; 
        }

        #endregion

        public void AddQueryInsertBillingTransaction(RDBQueryContext queryContext, BillingTransaction billingTransaction, long? createdByInvoiceId, bool generateId)
        {
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            if (generateId)
                insertQuery.GenerateIdAndAssignToParameter("BillingTransactionId");
            insertQuery.ColumnValue("AccountID", billingTransaction.AccountId);
            insertQuery.ColumnValue("AccountTypeID", billingTransaction.AccountTypeId);
            insertQuery.ColumnValue("Amount", billingTransaction.Amount);
            insertQuery.ColumnValue("CurrencyId", billingTransaction.CurrencyId);
            insertQuery.ColumnValue("TransactionTypeID", billingTransaction.TransactionTypeId);
            insertQuery.ColumnValue("TransactionTime", billingTransaction.TransactionTime);
            insertQuery.ColumnValue("Notes", billingTransaction.Notes);
            insertQuery.ColumnValue("Reference", billingTransaction.Reference);
            insertQuery.ColumnValue("SourceID", billingTransaction.SourceId);
            if (billingTransaction.Settings != null)
            {
                string serializedSettings = Vanrise.Common.Serializer.Serialize(billingTransaction.Settings);
                insertQuery.ColumnValue("Settings", serializedSettings);
            }
            if (createdByInvoiceId.HasValue)
                insertQuery.ColumnValue("CreatedByInvoiceID", createdByInvoiceId.Value);
        }

        private void AddBillingTransactionsToUpdateBalanceCondition(RDBConditionContext conditionContext)
        {
            var orCondition = conditionContext.Or();
            var andCondition1 = orCondition.And();
            andCondition1.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false);
            andCondition1.ConditionIfColumnNotNull("IsBalanceUpdated").EqualsCondition("IsBalanceUpdated", false);
            var andCondition2 = orCondition.Or();
            andCondition2.NotNullCondition("IsDeleted");
            andCondition2.EqualsCondition("IsDeleted", true);
            andCondition2.NotNullCondition("IsBalanceUpdated");
            andCondition2.EqualsCondition("IsBalanceUpdated", true);
            andCondition2.ConditionIfColumnNotNull("IsSubtractedFromBalance").EqualsCondition("IsSubtractedFromBalance", false);
        }

        public void AddQuerySetBillingTransactionsAsBalanceUpdated(RDBQueryContext queryContext, IEnumerable<long> billingTransactionIds)
        {
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(BillingTransactionDataManager.TABLE_NAME);
            updateQuery.ColumnValue("IsBalanceUpdated", true);
            updateQuery.Where().ListCondition("ID", RDBListConditionOperator.IN, billingTransactionIds);
        }

        public void AddQuerySetBillingTransactionsAsSubtractedFromBalance(RDBQueryContext queryContext, IEnumerable<long> billingTransactionIds)
        {
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.ColumnValue("IsSubtractedFromBalance", true);
            updateQuery.Where().ListCondition("ID", RDBListConditionOperator.IN, billingTransactionIds);
        }

        public void AddQuerySetInvoiceBillingTransactionsDeleted(RDBQueryContext queryContext, long invoiceId)
        {
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.ColumnValue("IsDeleted", true);
            var whereAndCondition = updateQuery.Where().And();
            whereAndCondition.NotNullCondition("CreatedByInvoiceID");
            whereAndCondition.EqualsCondition("CreatedByInvoiceID", invoiceId);
        }

    }
}

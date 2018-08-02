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
        static string TABLE_NAME = "VR_AccountBalance_BillingTransaction";
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
            
            var where = selectQuery.Where();
            where.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted").Value(false);
            if (query.AccountsIds != null && query.AccountsIds.Count() > 0)
                where.ListCondition("AccountID", RDBListConditionOperator.IN, query.AccountsIds);
            if (query.TransactionTypeIds != null && query.TransactionTypeIds.Count() > 0)
                where.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, query.TransactionTypeIds);
            where.GreaterOrEqualCondition("TransactionTime").Value(query.FromTime);
            if (query.ToTime.HasValue)
                where.LessOrEqualCondition("TransactionTime").Value(query.ToTime.Value);
            where.EqualsCondition("AccountTypeID").Value(query.AccountTypeId);
            liveBalanceDataManager.AddLiveBalanceActiveAndEffectiveCondition(where, "liveBalance", query.Status, query.EffectiveDate, query.IsEffectiveInFuture);
            
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

            var where = selectQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            AddBillingTransactionsToUpdateBalanceCondition(where);

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

            var where = selectQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            where.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted").Value(false);
            where.NotNullCondition("SourceID");
            if (billingTransactionTypeIds != null && billingTransactionTypeIds.Count() > 0)
                where.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, billingTransactionTypeIds);

            return queryContext.GetItems(BillingTransactionMapper);
        }

        public List<BillingTransactionMetaData> GetBillingTransactionsByAccountIds(Guid accountTypeId, List<Guid> transactionTypeIds, List<string> accountIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "bt");
            selectQuery.SelectColumns().Columns("AccountID", "Amount", "CurrencyId", "TransactionTime", "TransactionTypeID");

            var where = selectQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            where.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted").Value(false);
            if (accountIds != null && accountIds.Count() > 0)
                where.ListCondition("AccountID", RDBListConditionOperator.IN, accountIds);
            if (transactionTypeIds != null && transactionTypeIds.Count() > 0)
                where.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, transactionTypeIds);
            
            return queryContext.GetItems(BillingTransactionMetaDataMapper);
        }

        public IEnumerable<BillingTransactionMetaData> GetBillingTransactionsByTransactionTypes(Guid accountTypeId, List<BillingTransactionByTime> billingTransactionsByTime, List<Guid> transactionTypeIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var btAccountTimesTempTableQuery = queryContext.CreateTempTable();
            btAccountTimesTempTableQuery.AddColumn("AccountID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            btAccountTimesTempTableQuery.AddColumn("TransactionTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            
            foreach (var item in billingTransactionsByTime)
            {
                var insertQuery = queryContext.AddInsertQuery();
                insertQuery.IntoTable(btAccountTimesTempTableQuery);
                insertQuery.Column("AccountID").Value(item.AccountId);
                insertQuery.Column("TransactionTime").Value(item.TransactionTime);
            }

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "bt");
            selectQuery.SelectColumns().Columns("AccountID", "Amount", "CurrencyId", "TransactionTime", "TransactionTypeID");

            var joinContext = selectQuery.Join();
            var joinCondition = joinContext.Join(btAccountTimesTempTableQuery, "btAccountTimes").On();
            joinCondition.EqualsCondition("AccountID").Column("btAccountTimes", "AccountID");
            joinCondition.GreaterThanCondition("bt", "TransactionTime").Column("btAccountTimes", "TransactionTime");

            var where = selectQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            where.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted").Value(false);
            if (transactionTypeIds != null && transactionTypeIds.Count() > 0)
                where.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, transactionTypeIds);

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

            var where = selectQuery.Where();
            where.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted").Value(false);
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            where.EqualsCondition("AccountID").Value(accountId);
            liveBalanceDataManager.AddLiveBalanceActiveAndEffectiveCondition(where, "liveBalance", status, effectiveDate, isEffectiveInFuture);

            return queryContext.GetItems(BillingTransactionMapper);

        }

        public IEnumerable<BillingTransaction> GetBillingTransactions(List<Guid> accountTypeIds, List<string> accountIds, List<Guid> transactionTypeIds, DateTime fromDate, DateTime? toDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "bt");
            selectQuery.SelectColumns().AllTableColumns("bt");

            var where = selectQuery.Where();
            where.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted").Value(false);
            if (accountTypeIds != null && accountTypeIds.Count() > 0)
                where.ListCondition("AccountTypeID", RDBListConditionOperator.IN, accountTypeIds);
            if (accountIds != null && accountIds.Count() > 0)
                where.ListCondition("AccountID", RDBListConditionOperator.IN, accountIds);
            if (transactionTypeIds != null && transactionTypeIds.Count() > 0)
                where.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, transactionTypeIds);
            where.GreaterOrEqualCondition("TransactionTime").Value(fromDate);
            if (!toDate.HasValue)
                where.LessOrEqualCondition("TransactionTime").Value(toDate.Value);

            return queryContext.GetItems(BillingTransactionMapper);
        }

        public BillingTransaction GetLastBillingTransaction(Guid accountTypeId, string accountId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "bt", 1);
            selectQuery.SelectColumns().AllTableColumns("bt");

            var where = selectQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            where.EqualsCondition("AccountID").Value(accountId);
            where.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted").Value(false);

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

            var where = selectQuery.Where();
            where.EqualsCondition("AccountTypeID").Value(accountTypeId);
            AddBillingTransactionsToUpdateBalanceCondition(where);

            return queryContext.ExecuteScalar().NullableLongValue.HasValue; 
        }

        #endregion

        public void AddQueryInsertBillingTransaction(RDBQueryContext queryContext, BillingTransaction billingTransaction, long? createdByInvoiceId, bool generateId)
        {
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            if (generateId)
                insertQuery.GenerateIdAndAssignToParameter("BillingTransactionId");
            insertQuery.Column("AccountID").Value(billingTransaction.AccountId);
            insertQuery.Column("AccountTypeID").Value(billingTransaction.AccountTypeId);
            insertQuery.Column("Amount").Value(billingTransaction.Amount);
            insertQuery.Column("CurrencyId").Value(billingTransaction.CurrencyId);
            insertQuery.Column("TransactionTypeID").Value(billingTransaction.TransactionTypeId);
            insertQuery.Column("TransactionTime").Value(billingTransaction.TransactionTime);
            insertQuery.Column("Notes").Value(billingTransaction.Notes);
            insertQuery.Column("Reference").Value(billingTransaction.Reference);
            insertQuery.Column("SourceID").Value(billingTransaction.SourceId);
            if (billingTransaction.Settings != null)
            {
                string serializedSettings = Vanrise.Common.Serializer.Serialize(billingTransaction.Settings);
                insertQuery.Column("Settings").Value(serializedSettings);
            }
            if (createdByInvoiceId.HasValue)
                insertQuery.Column("CreatedByInvoiceID").Value(createdByInvoiceId.Value);
        }

        private void AddBillingTransactionsToUpdateBalanceCondition(RDBConditionContext conditionContext)
        {
            var orCondition = conditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            var andCondition1 = orCondition.ChildConditionGroup();
            andCondition1.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted").Value(false);
            andCondition1.ConditionIfColumnNotNull("IsBalanceUpdated").EqualsCondition("IsBalanceUpdated").Value(false);
            var andCondition2 = orCondition.ChildConditionGroup();
            andCondition2.NotNullCondition("IsDeleted");
            andCondition2.EqualsCondition("IsDeleted").Value(true);
            andCondition2.NotNullCondition("IsBalanceUpdated");
            andCondition2.EqualsCondition("IsBalanceUpdated").Value(true);
            andCondition2.ConditionIfColumnNotNull("IsSubtractedFromBalance").EqualsCondition("IsSubtractedFromBalance").Value(false);
        }

        public void AddQuerySetBillingTransactionsAsBalanceUpdated(RDBQueryContext queryContext, IEnumerable<long> billingTransactionIds)
        {
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(BillingTransactionDataManager.TABLE_NAME);
            updateQuery.Column("IsBalanceUpdated").Value(true);
            updateQuery.Where().ListCondition("ID", RDBListConditionOperator.IN, billingTransactionIds);
        }

        public void AddQuerySetBillingTransactionsAsSubtractedFromBalance(RDBQueryContext queryContext, IEnumerable<long> billingTransactionIds)
        {
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column("IsSubtractedFromBalance").Value(true);
            updateQuery.Where().ListCondition("ID", RDBListConditionOperator.IN, billingTransactionIds);
        }

        public void AddQuerySetInvoiceBillingTransactionsDeleted(RDBQueryContext queryContext, long invoiceId)
        {
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column("IsDeleted").Value(true);
            var where = updateQuery.Where();
            where.NotNullCondition("CreatedByInvoiceID");
            where.EqualsCondition("CreatedByInvoiceID").Value(invoiceId);
        }

    }
}

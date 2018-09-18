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

        const string COL_ID = "ID";
        const string COL_AccountTypeID = "AccountTypeID";
        const string COL_AccountID = "AccountID";
        const string COL_TransactionTypeID = "TransactionTypeID";
        const string COL_Amount = "Amount";
        const string COL_CurrencyId = "CurrencyId";
        const string COL_TransactionTime = "TransactionTime";
        const string COL_Reference = "Reference";
        const string COL_Notes = "Notes";
        const string COL_CreatedByInvoiceID = "CreatedByInvoiceID";
        const string COL_IsBalanceUpdated = "IsBalanceUpdated";
        const string COL_Settings = "Settings";
        const string COL_IsDeleted = "IsDeleted";
        const string COL_IsSubtractedFromBalance = "IsSubtractedFromBalance";
        const string COL_SourceID = "SourceID";
        const string COL_CreatedTime = "CreatedTime";

        static BillingTransactionDataManager()
        {
            var columns = new Dictionary<string,RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition{ DataType = RDBDataType.BigInt});
            columns.Add(COL_AccountTypeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier});
            columns.Add(COL_AccountID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_TransactionTypeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Amount, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6 });
            columns.Add(COL_CurrencyId, new RDBTableColumnDefinition {  DataType = RDBDataType.Int });
            columns.Add(COL_TransactionTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_Reference, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Notes, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 1000 });
            columns.Add(COL_CreatedByInvoiceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_IsBalanceUpdated, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_IsDeleted, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_IsSubtractedFromBalance, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_AccountBalance",
                DBTableName = "BillingTransaction",
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

        private BillingTransaction BillingTransactionMapper(IRDBDataReader reader)
        {
            string settingsAsString = reader.GetString(COL_Settings);

            return new BillingTransaction
            {
                AccountBillingTransactionId = reader.GetLong(COL_ID),
                AccountId = reader.GetString(COL_AccountID),
                AccountTypeId = reader.GetGuid(COL_AccountTypeID),
                Amount = reader.GetDecimal(COL_Amount),
                CurrencyId = reader.GetInt(COL_CurrencyId),
                Notes = reader.GetString(COL_Notes),
                TransactionTime = reader.GetDateTime(COL_TransactionTime),
                TransactionTypeId = reader.GetGuid(COL_TransactionTypeID),
                Reference = reader.GetString(COL_Reference),
                IsBalanceUpdated = reader.GetBooleanWithNullHandling(COL_IsBalanceUpdated),
                SourceId = reader.GetString(COL_SourceID),
                Settings = (settingsAsString != null) ? Vanrise.Common.Serializer.Deserialize<BillingTransactionSettings>(settingsAsString) : null,
                IsDeleted = reader.GetBooleanWithNullHandling(COL_IsDeleted),
                IsSubtractedFromBalance = reader.GetBooleanWithNullHandling(COL_IsSubtractedFromBalance)
            };
        }


        private BillingTransactionMetaData BillingTransactionMetaDataMapper(IRDBDataReader reader)
        {
            return new BillingTransactionMetaData
            {
                AccountId = reader.GetString(COL_AccountID),
                Amount = reader.GetDecimal(COL_Amount),
                CurrencyId = reader.GetInt(COL_CurrencyId),
                TransactionTime = reader.GetDateTime(COL_TransactionTime),
                TransactionTypeId = reader.GetGuid(COL_TransactionTypeID),
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
            liveBalanceDataManager.JoinLiveBalance(joinContext, "liveBalance", "bt", COL_AccountTypeID, COL_AccountID);
            
            var where = selectQuery.Where();
            where.ConditionIfColumnNotNull(COL_IsDeleted).EqualsCondition(COL_IsDeleted).Value(false);
            if (query.AccountsIds != null && query.AccountsIds.Count() > 0)
                where.ListCondition(COL_AccountID, RDBListConditionOperator.IN, query.AccountsIds);
            if (query.TransactionTypeIds != null && query.TransactionTypeIds.Count() > 0)
                where.ListCondition(COL_TransactionTypeID, RDBListConditionOperator.IN, query.TransactionTypeIds);
            where.GreaterOrEqualCondition(COL_TransactionTime).Value(query.FromTime);
            if (query.ToTime.HasValue)
                where.LessOrEqualCondition(COL_TransactionTime).Value(query.ToTime.Value);
            where.EqualsCondition(COL_AccountTypeID).Value(query.AccountTypeId);
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
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
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
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            where.ConditionIfColumnNotNull(COL_IsDeleted).EqualsCondition(COL_IsDeleted).Value(false);
            where.NotNullCondition(COL_SourceID);
            if (billingTransactionTypeIds != null && billingTransactionTypeIds.Count() > 0)
                where.ListCondition(COL_TransactionTypeID, RDBListConditionOperator.IN, billingTransactionTypeIds);

            return queryContext.GetItems(BillingTransactionMapper);
        }

        public List<BillingTransactionMetaData> GetBillingTransactionsByAccountIds(Guid accountTypeId, List<Guid> transactionTypeIds, List<string> accountIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "bt");
            selectQuery.SelectColumns().Columns(COL_AccountID, COL_Amount, COL_CurrencyId, COL_TransactionTime, COL_TransactionTypeID);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            where.ConditionIfColumnNotNull(COL_IsDeleted).EqualsCondition(COL_IsDeleted).Value(false);
            if (accountIds != null && accountIds.Count() > 0)
                where.ListCondition(COL_AccountID, RDBListConditionOperator.IN, accountIds);
            if (transactionTypeIds != null && transactionTypeIds.Count() > 0)
                where.ListCondition(COL_TransactionTypeID, RDBListConditionOperator.IN, transactionTypeIds);
            
            return queryContext.GetItems(BillingTransactionMetaDataMapper);
        }

        public IEnumerable<BillingTransactionMetaData> GetBillingTransactionsByTransactionTypes(Guid accountTypeId, List<BillingTransactionByTime> billingTransactionsByTime, List<Guid> transactionTypeIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var btAccountTimesTempTableQuery = queryContext.CreateTempTable();
            btAccountTimesTempTableQuery.AddColumn(COL_AccountID, RDBDataType.Varchar, 50, null, true);
            btAccountTimesTempTableQuery.AddColumn(COL_TransactionTime, RDBDataType.DateTime, true);
            
            foreach (var item in billingTransactionsByTime)
            {
                var insertQuery = queryContext.AddInsertQuery();
                insertQuery.IntoTable(btAccountTimesTempTableQuery);
                insertQuery.Column(COL_AccountID).Value(item.AccountId);
                insertQuery.Column(COL_TransactionTime).Value(item.TransactionTime);
            }

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "bt");
            selectQuery.SelectColumns().Columns(COL_AccountID, COL_Amount, COL_CurrencyId, COL_TransactionTime, COL_TransactionTypeID);

            var joinContext = selectQuery.Join();
            var joinCondition = joinContext.Join(btAccountTimesTempTableQuery, "btAccountTimes").On();
            joinCondition.EqualsCondition(COL_AccountID).Column("btAccountTimes", COL_AccountID);
            joinCondition.GreaterThanCondition("bt", COL_TransactionTime).Column("btAccountTimes", COL_TransactionTime);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            where.ConditionIfColumnNotNull(COL_IsDeleted).EqualsCondition(COL_IsDeleted).Value(false);
            if (transactionTypeIds != null && transactionTypeIds.Count() > 0)
                where.ListCondition(COL_TransactionTypeID, RDBListConditionOperator.IN, transactionTypeIds);

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
            liveBalanceDataManager.JoinLiveBalance(joinContext, "liveBalance", "bt", COL_AccountTypeID, COL_AccountID);

            var where = selectQuery.Where();
            where.ConditionIfColumnNotNull(COL_IsDeleted).EqualsCondition(COL_IsDeleted).Value(false);
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            where.EqualsCondition(COL_AccountID).Value(accountId);
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
            where.ConditionIfColumnNotNull(COL_IsDeleted).EqualsCondition(COL_IsDeleted).Value(false);
            if (accountTypeIds != null && accountTypeIds.Count() > 0)
                where.ListCondition(COL_AccountTypeID, RDBListConditionOperator.IN, accountTypeIds);
            if (accountIds != null && accountIds.Count() > 0)
                where.ListCondition(COL_AccountID, RDBListConditionOperator.IN, accountIds);
            if (transactionTypeIds != null && transactionTypeIds.Count() > 0)
                where.ListCondition(COL_TransactionTypeID, RDBListConditionOperator.IN, transactionTypeIds);
            where.GreaterOrEqualCondition(COL_TransactionTime).Value(fromDate);
            if (!toDate.HasValue)
                where.LessOrEqualCondition(COL_TransactionTime).Value(toDate.Value);

            return queryContext.GetItems(BillingTransactionMapper);
        }

        public BillingTransaction GetLastBillingTransaction(Guid accountTypeId, string accountId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "bt", 1);
            selectQuery.SelectColumns().AllTableColumns("bt");

            var where = selectQuery.Where();
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            where.EqualsCondition(COL_AccountID).Value(accountId);
            where.ConditionIfColumnNotNull(COL_IsDeleted).EqualsCondition(COL_IsDeleted).Value(false);

            var sortContext = selectQuery.Sort();
            sortContext.ByColumn(COL_TransactionTime, RDBSortDirection.DESC);

            return queryContext.GetItem(BillingTransactionMapper);
        }

        public bool HasBillingTransactionData(Guid accountTypeId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "bt", 1);
            selectQuery.SelectColumns().Column(COL_ID);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            AddBillingTransactionsToUpdateBalanceCondition(where);

            return queryContext.ExecuteScalar().NullableLongValue.HasValue; 
        }

        #endregion

        public void AddQueryInsertBillingTransaction(RDBQueryContext queryContext, BillingTransaction billingTransaction, long? createdByInvoiceId, bool addSelectGeneratedId)
        {
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            if (addSelectGeneratedId)
                insertQuery.AddSelectGeneratedId();
            insertQuery.Column(COL_AccountID).Value(billingTransaction.AccountId);
            insertQuery.Column(COL_AccountTypeID).Value(billingTransaction.AccountTypeId);
            insertQuery.Column(COL_Amount).Value(billingTransaction.Amount);
            insertQuery.Column(COL_CurrencyId).Value(billingTransaction.CurrencyId);
            insertQuery.Column(COL_TransactionTypeID).Value(billingTransaction.TransactionTypeId);
            insertQuery.Column(COL_TransactionTime).Value(billingTransaction.TransactionTime);
            insertQuery.Column(COL_Notes).Value(billingTransaction.Notes);
            insertQuery.Column(COL_Reference).Value(billingTransaction.Reference);
            insertQuery.Column(COL_SourceID).Value(billingTransaction.SourceId);
            if (billingTransaction.Settings != null)
            {
                string serializedSettings = Vanrise.Common.Serializer.Serialize(billingTransaction.Settings);
                insertQuery.Column(COL_Settings).Value(serializedSettings);
            }
            if (createdByInvoiceId.HasValue)
                insertQuery.Column(COL_CreatedByInvoiceID).Value(createdByInvoiceId.Value);
        }

        private void AddBillingTransactionsToUpdateBalanceCondition(RDBConditionContext conditionContext)
        {
            var orCondition = conditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            var andCondition1 = orCondition.ChildConditionGroup();
            andCondition1.ConditionIfColumnNotNull(COL_IsDeleted).EqualsCondition(COL_IsDeleted).Value(false);
            andCondition1.ConditionIfColumnNotNull(COL_IsBalanceUpdated).EqualsCondition(COL_IsBalanceUpdated).Value(false);
            var andCondition2 = orCondition.ChildConditionGroup();
            andCondition2.NotNullCondition(COL_IsDeleted);
            andCondition2.EqualsCondition(COL_IsDeleted).Value(true);
            andCondition2.NotNullCondition(COL_IsBalanceUpdated);
            andCondition2.EqualsCondition(COL_IsBalanceUpdated).Value(true);
            andCondition2.ConditionIfColumnNotNull(COL_IsSubtractedFromBalance).EqualsCondition(COL_IsSubtractedFromBalance).Value(false);
        }

        public void AddQuerySetBillingTransactionsAsBalanceUpdated(RDBQueryContext queryContext, IEnumerable<long> billingTransactionIds)
        {
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(BillingTransactionDataManager.TABLE_NAME);
            updateQuery.Column(COL_IsBalanceUpdated).Value(true);
            updateQuery.Where().ListCondition(COL_ID, RDBListConditionOperator.IN, billingTransactionIds);
        }

        public void AddQuerySetBillingTransactionsAsSubtractedFromBalance(RDBQueryContext queryContext, IEnumerable<long> billingTransactionIds)
        {
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_IsSubtractedFromBalance).Value(true);
            updateQuery.Where().ListCondition(COL_ID, RDBListConditionOperator.IN, billingTransactionIds);
        }

        public void AddQuerySetInvoiceBillingTransactionsDeleted(RDBQueryContext queryContext, long invoiceId)
        {
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_IsDeleted).Value(true);
            var where = updateQuery.Where();
            where.NotNullCondition(COL_CreatedByInvoiceID);
            where.EqualsCondition(COL_CreatedByInvoiceID).Value(invoiceId);
        }

        public BillingTransaction GetBillingTransactionById(long billingTransactionId)
        {
            throw new NotImplementedException();
        }
    }
}

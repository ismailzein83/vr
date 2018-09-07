using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
using Vanrise.AccountBalance.Data.RDB;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Data.RDB;

namespace Vanrise.Invoice.Data.RDB
{
    public class InvoiceDataManager : IInvoiceDataManager
    {
        static string TABLE_NAME = "VR_Invoice_Invoice";

        const string COL_ID = "ID";
        const string COL_UserId = "UserId";
        internal const string COL_InvoiceTypeID = "InvoiceTypeID";
        internal const string COL_PartnerID = "PartnerID";
        const string COL_SettlementInvoiceId = "SettlementInvoiceId";
        const string COL_SplitInvoiceGroupId = "SplitInvoiceGroupId";
        const string COL_InvoiceSettingID = "InvoiceSettingID";
        const string COL_SerialNumber = "SerialNumber";
        internal const string COL_FromDate = "FromDate";
        internal const string COL_ToDate = "ToDate";
        const string COL_IssueDate = "IssueDate";
        const string COL_DueDate = "DueDate";
        const string COL_Details = "Details";
        const string COL_PaidDate = "PaidDate";
        const string COL_LockDate = "LockDate";
        const string COL_SentDate = "SentDate";
        const string COL_IsDeleted = "IsDeleted";
        const string COL_Notes = "Notes";
        const string COL_Settings = "Settings";
        const string COL_SourceId = "SourceId";
        const string COL_IsDraft = "IsDraft";
        const string COL_IsAutomatic = "IsAutomatic";
        const string COL_NeedApproval = "NeedApproval";
        const string COL_ApprovedTime = "ApprovedTime";
        const string COL_ApprovedBy = "ApprovedBy";
        const string COL_CreatedTime = "CreatedTime";

        static InvoiceDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_UserId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_InvoiceTypeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_PartnerID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_SettlementInvoiceId, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_SplitInvoiceGroupId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_InvoiceSettingID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_SerialNumber, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_FromDate, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_ToDate, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_IssueDate, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_DueDate, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_Details, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_PaidDate, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LockDate, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_SentDate, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_IsDeleted, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_Notes, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_SourceId, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 50 });
            columns.Add(COL_IsDraft, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_IsAutomatic, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_NeedApproval, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_ApprovedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_ApprovedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_Invoice",
                DBTableName = "Invoice",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }

        #region Private Methods

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Invoice", "InvoiceDBConnStringKey", "InvoiceDBConnString");
        }

        string GetColumnNameFromFieldName(string fieldName)
        {
            switch (fieldName)
            {
                case "Partner": return COL_PartnerID;
                case "User": return COL_UserId;
                default: return fieldName;
            }
        }

        public Entities.Invoice InvoiceMapper(IRDBDataReader reader)
        {
            Entities.Invoice invoice = new Entities.Invoice
            {
                Details = Vanrise.Common.Serializer.Deserialize(reader.GetString(COL_Details)),
                FromDate = reader.GetDateTimeWithNullHandling(COL_FromDate),
                InvoiceId = reader.GetLong(COL_ID),
                InvoiceTypeId = reader.GetGuid(COL_InvoiceTypeID),
                IssueDate = reader.GetDateTimeWithNullHandling(COL_IssueDate),
                PartnerId = reader.GetString(COL_PartnerID),
                SerialNumber = reader.GetString(COL_SerialNumber),
                ToDate = reader.GetDateTimeWithNullHandling(COL_ToDate),
                PaidDate = reader.GetNullableDateTime(COL_PaidDate),
                DueDate = reader.GetDateTimeWithNullHandling(COL_DueDate),
                UserId = reader.GetIntWithNullHandling(COL_UserId),
                CreatedTime = reader.GetDateTimeWithNullHandling(COL_CreatedTime),
                LockDate = reader.GetNullableDateTime(COL_LockDate),
                Note = reader.GetString(COL_Notes),
                SourceId = reader.GetString(COL_SourceId),
                IsAutomatic = reader.GetBooleanWithNullHandling(COL_IsAutomatic),
                Settings = Vanrise.Common.Serializer.Deserialize<InvoiceSettings>(reader.GetString(COL_Settings)),
                InvoiceSettingId = reader.GetGuidWithNullHandling(COL_InvoiceSettingID),
                SentDate = reader.GetNullableDateTime(COL_SentDate),
                SettlementInvoiceId = reader.GetNullableLong(COL_SettlementInvoiceId),
                SplitInvoiceGroupId = reader.GetNullableGuid(COL_SplitInvoiceGroupId),
                ApprovedBy = reader.GetNullableInt(COL_ApprovedBy),
                ApprovedTime = reader.GetNullableDateTime(COL_ApprovedTime),
                NeedApproval = reader.GetNullableBoolean(COL_NeedApproval),
            };
            return invoice;
        }

        public InvoiceByPartnerInfo InvoiceByPartnerInfoMapper(IRDBDataReader reader)
        {
            return new InvoiceByPartnerInfo
            {
                InvoiceTypeId = reader.GetGuidWithNullHandling(COL_InvoiceTypeID),
                IssueDate = reader.GetDateTimeWithNullHandling(COL_IssueDate),
                PartnerId = reader.GetString(COL_PartnerID),
                ToDate = reader.GetDateTimeWithNullHandling(COL_ToDate),
                DueDate = reader.GetDateTimeWithNullHandling(COL_DueDate),
            };
        }
        public VRPopulatedPeriod VRPopulatedPeriodMapper(IRDBDataReader reader)
        {
            return new VRPopulatedPeriod
            {
                FromDate = reader.GetNullableDateTime(COL_FromDate),
                ToDate = reader.GetNullableDateTime(COL_ToDate),
            };
        }

        #endregion

        #region IInvoiceDataManager

        public List<Entities.Invoice> GetPartnerInvoicesByDate(Guid invoiceTypeId, IEnumerable<string> partnerIds, DateTime fromDate, DateTime toDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv");
            selectQuery.SelectColumns().AllTableColumns("inv");

            var where = selectQuery.Where();
            where.EqualsCondition(COL_InvoiceTypeID).Value(invoiceTypeId);
            if (partnerIds != null && partnerIds.Count() > 0)
                where.ListCondition(COL_PartnerID, RDBListConditionOperator.IN, partnerIds);
            where.GreaterThanCondition(COL_FromDate).Value(fromDate);
            where.LessThanCondition(COL_ToDate).Value(toDate);
            AddConditionInvoiceNotDeleted(where);

            return queryContext.GetItems(InvoiceMapper);
        }

        public IEnumerable<Entities.Invoice> GetFilteredInvoices(Vanrise.Entities.DataRetrievalInput<InvoiceQuery> input)
        {
            var query = input.Query;
            InvoiceAccountDataManager invoiceAccountDataManager = new InvoiceAccountDataManager();
            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumnsFromTable(TABLE_NAME);

            var insertIntoTempTableQuery = queryContext.AddInsertQuery();
            insertIntoTempTableQuery.IntoTable(tempTableQuery);

            var selectInvoicesQuery = insertIntoTempTableQuery.FromSelect();
            selectInvoicesQuery.From(TABLE_NAME, "inv");
            selectInvoicesQuery.SelectColumns().AllTableColumns("inv");

            invoiceAccountDataManager.AddJoinInvoiceToInvoiceAccount(selectInvoicesQuery.Join(), "inv", "invAcc", query.Status, query.EffectiveDate, query.IsEffectiveInFuture);

            var where = selectInvoicesQuery.Where();
            where.EqualsCondition(COL_InvoiceTypeID).Value(query.InvoiceTypeId);
            if (query.PartnerIds != null && query.PartnerIds.Count > 0)
                where.ListCondition(COL_PartnerID, RDBListConditionOperator.IN, query.PartnerIds);
            if (query.PartnerPrefix != null)
                where.StartsWithCondition(COL_PartnerID, query.PartnerPrefix);
            where.GreaterOrEqualCondition(COL_FromDate).Value(query.FromTime);
            if (query.ToTime.HasValue)
                where.LessOrEqualCondition(COL_ToDate).Value(query.ToTime.Value);
            if (query.IssueDate.HasValue)
                where.EqualsCondition(COL_IssueDate).Value(query.IssueDate.Value);
            AddConditionInvoiceNotDeleted(where);
            if(query.IsPaid.HasValue)
            {
                if (query.IsPaid.Value)
                    where.NotNullCondition(COL_PaidDate);
                else
                    where.NullCondition(COL_PaidDate);
            }
            if(query.IssueDate.HasValue)
            {
                if (query.IsSent.Value)
                    where.NotNullCondition(COL_SentDate);
                else
                    where.NullCondition(COL_SentDate);
            }

            InvoiceBulkActionsDraftDataManager invoiceBulkActionsDraftDataManager = new InvoiceBulkActionsDraftDataManager();

            if(query.IsSelectAll.HasValue && query.InvoiceBulkActionIdentifier.HasValue)
            {
                if(query.IsSelectAll.Value)
                {
                    var insertDraftsQuery = invoiceBulkActionsDraftDataManager.CreateInsertQuery(queryContext);
                    var selectDraftsToInsertQuery = insertDraftsQuery.FromSelect();
                    selectDraftsToInsertQuery.From(tempTableQuery, "tmp");
                    var selectColumns = selectDraftsToInsertQuery.SelectColumns();
                    selectColumns.Column(COL_InvoiceTypeID, InvoiceBulkActionsDraftDataManager.COL_InvoiceTypeId);
                    selectColumns.Column(COL_ID, InvoiceBulkActionsDraftDataManager.COL_InvoiceId);
                    selectColumns.Expression("InvoiceBulkActionIdentifier").Value(query.InvoiceBulkActionIdentifier.Value);
                }
            }


            var selectFromTempQuery = queryContext.AddSelectQuery();
            selectFromTempQuery.From(tempTableQuery, "tmp");
            selectFromTempQuery.SelectColumns().AllTableColumns("tmp");

            var invoices = queryContext.GetItems(InvoiceMapper);

            if(query.IsSelectAll.HasValue && !query.IsSelectAll.Value && query.InvoiceBulkActionIdentifier.HasValue)
            {
                invoiceBulkActionsDraftDataManager.DeleteBulkActionDrafts(query.InvoiceBulkActionIdentifier.Value); 
            }

            return invoices;
        }

        public int GetInvoiceCount(Guid InvoiceTypeId, string partnerId, DateTime? fromDate, DateTime? toDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv");
            selectQuery.SelectAggregates().Count("cnt");

            var where = selectQuery.Where();
            where.EqualsCondition(COL_InvoiceTypeID).Value(InvoiceTypeId);
            AddConditionInvoiceNotDeleted(where);
            if (partnerId != null)
                where.EqualsCondition(COL_PartnerID).Value(partnerId);
            if (fromDate.HasValue)
                where.GreaterOrEqualCondition(COL_IssueDate).Value(fromDate.Value);
            if (toDate.HasValue)
                where.LessOrEqualCondition(COL_IssueDate).Value(toDate.Value);

            return queryContext.ExecuteScalar().IntValue;
        }

        public bool SaveInvoices(List<GenerateInvoiceInputToSave> generateInvoicesInputToSave, out List<long> insertedInvoiceIds)
        {
            BillingTransactionDataManager billingTransactionDataManager = new BillingTransactionDataManager();
            insertedInvoiceIds = null;
            if (generateInvoicesInputToSave == null || generateInvoicesInputToSave.Count == 0)
                return false;
            var dataProvider = GetDataProvider();
            insertedInvoiceIds = new List<long>();

            Guid? splitInvoiceGroupId = Guid.NewGuid();

            for (var i = 0; i < generateInvoicesInputToSave.Count; i++)
            {
                var generateInvoiceInputToSave = generateInvoicesInputToSave[i];

                if (generateInvoiceInputToSave.InvoiceIdToDelete.HasValue && generateInvoiceInputToSave.SplitInvoiceGroupId.HasValue)
                {
                    splitInvoiceGroupId = generateInvoiceInputToSave.SplitInvoiceGroupId.Value;
                }

                string serializedSettings = null;
                if (generateInvoiceInputToSave.Invoice.Settings != null)
                {
                    serializedSettings = Vanrise.Common.Serializer.Serialize(generateInvoiceInputToSave.Invoice.Settings);
                }
                string serializedDetails = Vanrise.Common.Serializer.Serialize(generateInvoiceInputToSave.Invoice.Details);
                var queryContext = new RDBQueryContext(dataProvider);
                var insertQuery = queryContext.AddInsertQuery();
                insertQuery.IntoTable(TABLE_NAME);
                insertQuery.AddSelectGeneratedId();
                insertQuery.Column(COL_UserId).Value(generateInvoiceInputToSave.Invoice.UserId);
                insertQuery.Column(COL_InvoiceTypeID).Value(generateInvoiceInputToSave.Invoice.InvoiceTypeId);
                insertQuery.Column(COL_PartnerID).Value(generateInvoiceInputToSave.Invoice.PartnerId);
                if (splitInvoiceGroupId.HasValue)
                    insertQuery.Column(COL_SplitInvoiceGroupId).Value(splitInvoiceGroupId.Value);
                insertQuery.Column(COL_InvoiceSettingID).Value(generateInvoiceInputToSave.Invoice.InvoiceSettingId);
                insertQuery.Column(COL_SerialNumber).Value(generateInvoiceInputToSave.Invoice.SerialNumber);
                insertQuery.Column(COL_FromDate).Value(generateInvoiceInputToSave.Invoice.FromDate);
                insertQuery.Column(COL_ToDate).Value(generateInvoiceInputToSave.Invoice.ToDate);
                insertQuery.Column(COL_IssueDate).Value(generateInvoiceInputToSave.Invoice.IssueDate);
                insertQuery.Column(COL_DueDate).Value(generateInvoiceInputToSave.Invoice.DueDate);
                insertQuery.Column(COL_Details).Value(serializedDetails);
                insertQuery.Column(COL_Notes).Value(generateInvoiceInputToSave.Invoice.Note);
                insertQuery.Column(COL_SourceId).Value(generateInvoiceInputToSave.Invoice.SourceId);
                insertQuery.Column(COL_IsDraft).Value(true);
                insertQuery.Column(COL_IsDeleted).Value(false);
                insertQuery.Column(COL_IsAutomatic).Value(generateInvoiceInputToSave.Invoice.IsAutomatic);
                insertQuery.Column(COL_Settings).Value(serializedSettings);
                if (generateInvoiceInputToSave.Invoice.SentDate.HasValue)
                    insertQuery.Column(COL_SentDate).Value(generateInvoiceInputToSave.Invoice.SentDate.Value);
                if (generateInvoiceInputToSave.Invoice.NeedApproval.HasValue)
                    insertQuery.Column(COL_NeedApproval).Value(generateInvoiceInputToSave.Invoice.NeedApproval.Value);
                long insertedInvoiceId = queryContext.ExecuteScalar().LongValue;

                insertedInvoiceIds.Add(insertedInvoiceId);

                if (generateInvoiceInputToSave.ItemSetNameStorageDic != null && generateInvoiceInputToSave.ItemSetNameStorageDic.Count > 0)
                {
                    var remainingInvoiceItemSets = generateInvoiceInputToSave.InvoiceItemSets.FindAllRecords(x => !generateInvoiceInputToSave.ItemSetNameStorageDic.Values.Any(y => y.Contains(x.SetName)));
                    if (remainingInvoiceItemSets != null)
                    {
                        InvoiceItemDataManager dataManager = new InvoiceItemDataManager();
                        dataManager.SaveInvoiceItems(insertedInvoiceId, remainingInvoiceItemSets);
                    }
                    foreach (var item in generateInvoiceInputToSave.ItemSetNameStorageDic)
                    {
                        InvoiceItemDataManager dataManager = new InvoiceItemDataManager();
                        dataManager.StorageConnectionStringKey = item.Key;
                        var invoiceItemSetsToSave = generateInvoiceInputToSave.InvoiceItemSets.FindAllRecords(x => item.Value.Contains(x.SetName));
                        dataManager.SaveInvoiceItems(insertedInvoiceId, invoiceItemSetsToSave);
                    }
                }
                else
                {
                    InvoiceItemDataManager dataManager = new InvoiceItemDataManager();
                    dataManager.SaveInvoiceItems(insertedInvoiceId, generateInvoiceInputToSave.InvoiceItemSets);
                }
                generateInvoiceInputToSave.Invoice.InvoiceId = insertedInvoiceId;

                if (generateInvoiceInputToSave.ActionBeforeGenerateInvoice != null)
                {
                    generateInvoiceInputToSave.ActionBeforeGenerateInvoice(generateInvoiceInputToSave.Invoice);
                }

                if (generateInvoiceInputToSave.ActionAfterGenerateInvoice != null)
                {
                    generateInvoiceInputToSave.ActionAfterGenerateInvoice(generateInvoiceInputToSave.Invoice);
                }
            }

            var queryContext2 = new RDBQueryContext(dataProvider);

            foreach (var generateInvoiceInputToSave in generateInvoicesInputToSave)
            {
                if (generateInvoiceInputToSave.InvoiceToSettleIds != null && generateInvoiceInputToSave.InvoiceToSettleIds.Count > 0)
                {
                    var updateQuery = queryContext2.AddUpdateQuery();
                    updateQuery.FromTable(TABLE_NAME);
                    updateQuery.Column(COL_SettlementInvoiceId).Value(generateInvoiceInputToSave.Invoice.InvoiceId);
                    updateQuery.Where().ListCondition(COL_ID, RDBListConditionOperator.IN, generateInvoiceInputToSave.InvoiceToSettleIds);

                    if (generateInvoiceInputToSave.InvoiceIdToDelete.HasValue)
                    {
                        var updateQuery2 = queryContext2.AddUpdateQuery();
                        updateQuery2.FromTable(TABLE_NAME);
                        updateQuery2.Column(COL_SettlementInvoiceId).Null();
                        updateQuery2.Where().EqualsCondition(COL_SettlementInvoiceId).Value(generateInvoiceInputToSave.InvoiceIdToDelete.Value);
                    }
                }

                if (generateInvoiceInputToSave.MappedTransactions != null && generateInvoiceInputToSave.MappedTransactions.Count() > 0)
                {
                    foreach (var billingTransaction in generateInvoiceInputToSave.MappedTransactions)
                    {
                        billingTransactionDataManager.AddQueryInsertBillingTransaction(queryContext2, billingTransaction, generateInvoiceInputToSave.Invoice.InvoiceId, false);
                    }
                }

                if (generateInvoiceInputToSave.InvoiceIdToDelete.HasValue)
                {
                    var updateQuery = queryContext2.AddUpdateQuery();
                    updateQuery.FromTable(TABLE_NAME);
                    updateQuery.Column(COL_IsDeleted).Value(true);
                    updateQuery.Where().EqualsCondition(COL_ID).Value(generateInvoiceInputToSave.InvoiceIdToDelete.Value);

                    billingTransactionDataManager.AddQuerySetInvoiceBillingTransactionsDeleted(queryContext2, generateInvoiceInputToSave.InvoiceIdToDelete.Value);
                }

                var updateInvoiceQuery = queryContext2.AddUpdateQuery();
                updateInvoiceQuery.FromTable(TABLE_NAME);
                updateInvoiceQuery.Column(COL_IsDraft).Value(false);
                updateInvoiceQuery.Where().EqualsCondition(COL_ID).Value(generateInvoiceInputToSave.Invoice.InvoiceId);
            }

            queryContext2.ExecuteNonQuery(true);

            return true;
        }

        public List<Entities.Invoice> GetInvoices(List<long> invoiceIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv");
            selectQuery.SelectColumns().AllTableColumns("inv");

            selectQuery.Where().ListCondition(COL_ID, RDBListConditionOperator.IN, invoiceIds);

            return queryContext.GetItems(InvoiceMapper);
        }

        public bool CheckInvoiceOverlaping(Guid invoiceTypeId, string partnerId, DateTime fromDate, DateTime toDate, long? invoiceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());            
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv", 1);
            selectQuery.SelectColumns().Column(COL_ID);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_InvoiceTypeID).Value(invoiceTypeId);
            where.EqualsCondition(COL_PartnerID).Value(partnerId);
            where.GreaterOrEqualCondition(COL_ToDate).Value(fromDate);
            where.LessOrEqualCondition(COL_FromDate).Value(toDate);
            AddConditionInvoiceNotDeleted(where);
            if (invoiceId.HasValue)
                where.NotEqualsCondition(COL_ID).Value(invoiceId.Value);

            return queryContext.ExecuteScalar().NullableLongValue.HasValue;
        }

        public bool SetInvoicePaid(long invoiceId, DateTime? paidDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            if (paidDate.HasValue)
                updateQuery.Column(COL_PaidDate).Value(paidDate.Value);
            else
                updateQuery.Column(COL_PaidDate).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(invoiceId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool SetInvoiceLocked(long invoiceId, DateTime? lockedDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            if (lockedDate.HasValue)
                updateQuery.Column(COL_LockDate).Value(lockedDate.Value);
            else
                updateQuery.Column(COL_LockDate).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(invoiceId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdateInvoiceNote(long invoiceId, string invoiceNote)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Notes).Value(invoiceNote);
            updateQuery.Where().EqualsCondition(COL_ID).Value(invoiceId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public void LoadInvoicesAfterImportedId(Guid invoiceTypeId, long lastImportedId, Action<Entities.Invoice> onInvoiceReady)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv");
            selectQuery.SelectColumns().AllTableColumns("inv");

            var where = selectQuery.Where();
            where.EqualsCondition(COL_InvoiceTypeID).Value(invoiceTypeId);
            AddConditionInvoiceNotDeleted(where);
            where.GreaterThanCondition(COL_ID).Value(lastImportedId);

            selectQuery.Sort().ByColumn(COL_ID, RDBSortDirection.ASC);

            queryContext.ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    onInvoiceReady(InvoiceMapper(reader));
                }
            });
        }

        public IEnumerable<Entities.Invoice> GetUnPaidPartnerInvoices(IEnumerable<PartnerInvoiceType> partnerInvoiceTypes)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(COL_InvoiceTypeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier }, true);
            tempTableQuery.AddColumn(COL_PartnerID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 }, true);

            if(partnerInvoiceTypes != null)
            {
                foreach(var partner in partnerInvoiceTypes)
                {
                    var insertQuery = queryContext.AddInsertQuery();
                    insertQuery.IntoTable(tempTableQuery);
                    insertQuery.Column(COL_InvoiceTypeID).Value(partner.InvoiceTypeId);
                    insertQuery.Column(COL_PartnerID).Value(partner.PartnerId);
                }
            }

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv");
            selectQuery.SelectColumns().AllTableColumns("inv");

            var joinTempCondition = selectQuery.Join().Join(tempTableQuery, "partners").On();
            joinTempCondition.EqualsCondition("inv", COL_InvoiceTypeID, "partners", COL_InvoiceTypeID);
            joinTempCondition.EqualsCondition("inv", COL_PartnerID, "partners", COL_PartnerID);

            var where = selectQuery.Where();
            where.NullCondition().Column(COL_PaidDate);
            where.LessOrEqualCondition(COL_DueDate).DateNow();
            AddConditionInvoiceNotDeleted(where);

            return queryContext.GetItems(InvoiceMapper);
        }

        public IEnumerable<InvoiceByPartnerInfo> GetLastInvoicesByPartners(IEnumerable<PartnerInvoiceType> partnerInvoiceTypes)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(COL_InvoiceTypeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier }, true);
            tempTableQuery.AddColumn(COL_PartnerID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 }, true);

            if (partnerInvoiceTypes != null)
            {
                foreach (var partner in partnerInvoiceTypes)
                {
                    var insertQuery = queryContext.AddInsertQuery();
                    insertQuery.IntoTable(tempTableQuery);
                    insertQuery.Column(COL_InvoiceTypeID).Value(partner.InvoiceTypeId);
                    insertQuery.Column(COL_PartnerID).Value(partner.PartnerId);
                }
            }

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv");
            selectQuery.SelectColumns().Columns(COL_InvoiceTypeID, COL_PartnerID, COL_IssueDate, COL_ToDate, COL_DueDate);

            var joinTempCondition = selectQuery.Join().Join(tempTableQuery, "partners").On();
            joinTempCondition.EqualsCondition("inv", COL_InvoiceTypeID, "partners", COL_InvoiceTypeID);
            joinTempCondition.EqualsCondition("inv", COL_PartnerID, "partners", COL_PartnerID);

            var where = selectQuery.Where();
            where.LessOrEqualCondition(COL_DueDate).DateNow();
            AddConditionInvoiceNotDeleted(where);

            return queryContext.GetItems(InvoiceByPartnerInfoMapper);
        }

        public bool Update(Entities.Invoice invoice)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();

            updateQuery.Column(COL_InvoiceTypeID).Value(invoice.InvoiceTypeId);
            updateQuery.Column(COL_PartnerID).Value(invoice.InvoiceTypeId);
            updateQuery.Column(COL_SerialNumber).Value(invoice.SerialNumber);
            updateQuery.Column(COL_FromDate).Value(invoice.FromDate);
            updateQuery.Column(COL_ToDate).Value(invoice.ToDate);
            updateQuery.Column(COL_IssueDate).Value(invoice.IssueDate);
            updateQuery.Column(COL_DueDate).Value(invoice.DueDate);
            updateQuery.Column(COL_Details).Value(Vanrise.Common.Serializer.Serialize(invoice.Details));
            if (invoice.PaidDate.HasValue)
                updateQuery.Column(COL_PaidDate).Value(invoice.PaidDate.Value);
            else
                updateQuery.Column(COL_PaidDate).Null();
            if (invoice.LockDate.HasValue)
                updateQuery.Column(COL_LockDate).Value(invoice.LockDate.Value);
            else
                updateQuery.Column(COL_LockDate).Null();
            updateQuery.Column(COL_Notes).Value(invoice.Note);
            updateQuery.Column(COL_SourceId).Value(invoice.SourceId);
            updateQuery.Column(COL_InvoiceSettingID).Value(invoice.InvoiceSettingId);
            if (invoice.SentDate.HasValue)
                updateQuery.Column(COL_SentDate).Value(invoice.SentDate.Value);
            else
                updateQuery.Column(COL_SentDate).Null();

            updateQuery.Where().EqualsCondition(COL_ID).Value(invoice.InvoiceId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public Entities.Invoice GetInvoiceBySourceId(Guid invoiceTypeId, string sourceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv");
            selectQuery.SelectColumns().AllTableColumns("inv");

            var where = selectQuery.Where();
            where.EqualsCondition(COL_InvoiceTypeID).Value(invoiceTypeId);
            where.EqualsCondition(COL_SourceId).Value(sourceId);
            AddConditionInvoiceNotDeleted(where);

            return queryContext.GetItem(InvoiceMapper);
        }

        public Entities.Invoice GetLastInvoice(Guid invoiceTypeId, string partnerId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv", 1);
            selectQuery.SelectColumns().AllTableColumns("inv");

            var where = selectQuery.Where();
            where.EqualsCondition(COL_InvoiceTypeID).Value(invoiceTypeId);
            where.EqualsCondition(COL_PartnerID).Value(partnerId);
            AddConditionInvoiceNotDeleted(where);

            selectQuery.Sort().ByColumn(COL_ID, RDBSortDirection.DESC);

            return queryContext.GetItem(InvoiceMapper);
        }

        public bool UpdateInvoicePaidDateBySourceId(Guid invoiceTypeId, string sourceId, DateTime paidDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column(COL_PaidDate).Value(paidDate);

            var where = updateQuery.Where();
            where.EqualsCondition(COL_InvoiceTypeID).Value(invoiceTypeId);
            where.EqualsCondition(COL_SourceId).Value(sourceId);
            AddConditionInvoiceNotDeleted(where);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public IEnumerable<Entities.Invoice> GetLasInvoices(Guid invoiceTypeId, string partnerId, DateTime? beforeDate, int lastInvoices)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv", lastInvoices);
            selectQuery.SelectColumns().AllTableColumns("inv");

            var where = selectQuery.Where();
            where.EqualsCondition(COL_InvoiceTypeID).Value(invoiceTypeId);
            where.EqualsCondition(COL_PartnerID).Value(partnerId);
            AddConditionInvoiceNotDeleted(where);
            if (beforeDate.HasValue)
                where.LessThanCondition(COL_CreatedTime).Value(beforeDate.Value);

            selectQuery.Sort().ByColumn(COL_ID, RDBSortDirection.DESC);

            return queryContext.GetItems(InvoiceMapper);
        }

        public VRPopulatedPeriod GetInvoicesPopulatedPeriod(Guid invoiceTypeId, string partnerId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv");

            var aggregates = selectQuery.SelectAggregates();
            aggregates.Aggregate(RDBNonCountAggregateType.MIN, COL_FromDate);
            aggregates.Aggregate(RDBNonCountAggregateType.MAX, COL_ToDate);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_InvoiceTypeID).Value(invoiceTypeId);
            where.EqualsCondition(COL_PartnerID).Value(partnerId);
            AddConditionInvoiceNotDeleted(where);

            return queryContext.GetItem(VRPopulatedPeriodMapper);
        }

        public bool CheckPartnerIfHasInvoices(Guid invoiceTypeId, string partnerId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv", 1);
            selectQuery.SelectColumns().Column(COL_ID);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_InvoiceTypeID).Value(invoiceTypeId);
            where.EqualsCondition(COL_PartnerID).Value(partnerId);

            return queryContext.ExecuteScalar().NullableLongValue.HasValue;
        }

        public List<Entities.Invoice> GetInvoicesBySerialNumbers(Guid invoiceTypeId, IEnumerable<string> serialNumbers)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv");
            selectQuery.SelectColumns().AllTableColumns("inv");

            var where = selectQuery.Where();
            where.EqualsCondition(COL_InvoiceTypeID).Value(invoiceTypeId);
            AddConditionInvoiceNotDeleted(where);
            where.ListCondition(COL_SerialNumber, RDBListConditionOperator.IN, serialNumbers);

            return queryContext.GetItems(InvoiceMapper);
        }

        public bool UpdateInvoicePaidDateById(Guid invoiceTypeId, long invoiceId, DateTime paidDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column(COL_PaidDate).Value(paidDate);

            var where = updateQuery.Where();
            where.EqualsCondition(COL_InvoiceTypeID).Value(invoiceTypeId);
            where.EqualsCondition(COL_ID).Value(invoiceId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdateInvoiceSettings(long invoiceId, InvoiceSettings invoiceSettings)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            string serializedSettings = null;
            if (invoiceSettings != null)
            {
                serializedSettings = Vanrise.Common.Serializer.Serialize(invoiceSettings);
            }
            updateQuery.Column(COL_Settings).Value(serializedSettings);

            var where = updateQuery.Where();
            where.EqualsCondition(COL_ID).Value(invoiceId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool SetInvoiceSentDate(long invoiceId, DateTime? sentDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            if (sentDate.HasValue)
                updateQuery.Column(COL_SentDate).Value(sentDate.Value);
            else
                updateQuery.Column(COL_SentDate).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(invoiceId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool DeleteGeneratedInvoice(long invoiceId, Guid invoiceTypeId, string partnerId, DateTime fromDate, DateTime toDate)
        {
            BillingTransactionDataManager billingTransactionDataManager = new BillingTransactionDataManager();
            BillingPeriodInfoDataManager billingPeriodInfoDataManager = new BillingPeriodInfoDataManager();
            var billingPeriodInfo = billingPeriodInfoDataManager.GetBillingPeriodInfoById(partnerId, invoiceTypeId);
            var nextPeriodStart = toDate.AddDays(1);

            var queryContext = new RDBQueryContext(GetDataProvider());


            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_IsDeleted).Value(true);
            updateQuery.Where().EqualsCondition(COL_ID).Value(invoiceId);

            if (billingPeriodInfo.NextPeriodStart.Date == nextPeriodStart.Date)
            {
                billingPeriodInfoDataManager.AddInsertOrUpdateBillingPeriodInfo(queryContext, new BillingPeriodInfo
                            {
                                InvoiceTypeId = invoiceTypeId,
                                NextPeriodStart = fromDate,
                                PartnerId = partnerId
                            });
            }

            billingTransactionDataManager.AddQuerySetInvoiceBillingTransactionsDeleted(queryContext, invoiceId);

            queryContext.ExecuteNonQuery(true);
            return true;
        }

        public void LoadInvoices(Guid invoiceTypeId, DateTime? from, DateTime? to, Vanrise.GenericData.Entities.RecordFilterGroup filterGroup, Vanrise.GenericData.Entities.OrderDirection? orderDirection, Func<bool> shouldStop, Action<Entities.Invoice> onInvoiceLoaded)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv");
            selectQuery.SelectColumns().AllTableColumns("inv");

            var where = selectQuery.Where();
            where.EqualsCondition(COL_InvoiceTypeID).Value(invoiceTypeId);
            if (from.HasValue)
                where.GreaterOrEqualCondition(COL_CreatedTime).Value(from.Value);
            if (to.HasValue)
                where.LessOrEqualCondition(COL_CreatedTime).Value(to.Value);
            if (filterGroup != null)
                new RecordFilterRDBBuilder((fieldName, expressionContext) => expressionContext.Column("inv", GetColumnNameFromFieldName(fieldName))).RecordFilterGroupCondition(where, filterGroup);

            if (orderDirection.HasValue)
                selectQuery.Sort().ByColumn(COL_ID, orderDirection.Value == OrderDirection.Ascending ? RDBSortDirection.ASC : RDBSortDirection.DESC);

            queryContext.ExecuteReader(
                        (reader) =>
                        {
                            while (reader.Read())
                            {
                                onInvoiceLoaded(InvoiceMapper(reader));
                                if (shouldStop())
                                    break;
                            }
                        }
                    );
        }
        public bool ApproveInvoice(long invoiceId, DateTime? ApprovedDate, int? ApprovedBy)
        {
            throw new NotImplementedException();
        }
        #endregion

        private void AddConditionInvoiceNotDeleted(RDBConditionContext conditionContext, string invoiceTableAlias = null)
        {
            if (invoiceTableAlias == null)
            {
                conditionContext.ConditionIfColumnNotNull(COL_IsDeleted).EqualsCondition(COL_IsDeleted).Value(false);
                conditionContext.ConditionIfColumnNotNull(COL_IsDraft).EqualsCondition(COL_IsDraft).Value(false);
            }
            else
            {
                conditionContext.ConditionIfColumnNotNull(invoiceTableAlias, COL_IsDeleted).EqualsCondition(invoiceTableAlias, COL_IsDeleted).Value(false);
                conditionContext.ConditionIfColumnNotNull(invoiceTableAlias, COL_IsDraft).EqualsCondition(invoiceTableAlias, COL_IsDraft).Value(false);
            }
        }

        internal void AddJoinInvoiceToBulkActionDraft(RDBJoinContext joinContext, string bulkActionDraftAlias, string invoiceAlias)
        {
            var joinCondition = joinContext.Join(TABLE_NAME, invoiceAlias).On();
            joinCondition.EqualsCondition(invoiceAlias, COL_InvoiceTypeID).Column(bulkActionDraftAlias, InvoiceBulkActionsDraftDataManager.COL_InvoiceTypeId);
            joinCondition.EqualsCondition(invoiceAlias, COL_ID).Column(bulkActionDraftAlias, InvoiceBulkActionsDraftDataManager.COL_InvoiceId);
            //AddConditionInvoiceNotDeleted(joinCondition, invoiceAlias);
        }
    }
}

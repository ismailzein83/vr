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

        static InvoiceDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add("ID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("UserId", new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add("InvoiceTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("PartnerID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add("SettlementInvoiceId", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("SplitInvoiceGroupId", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("InvoiceSettingID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("SerialNumber", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add("FromDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("ToDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("IssueDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("DueDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("Details", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add("PaidDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("LockDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("SentDate", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("IsDeleted", new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add("Notes", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add("Settings", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add("SourceId", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 50 });
            columns.Add("IsDraft", new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add("IsAutomatic", new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add("NeedApproval", new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add("ApprovedTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("ApprovedBy", new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add("CreatedTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_Invoice",
                DBTableName = "Invoice",
                Columns = columns,
                IdColumnName = "ID",
                CreatedTimeColumnName = "CreatedTime"
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
                case "Partner": return "PartnerID";
                case "User": return "UserId";
                default: return fieldName;
            }
        }

        public Entities.Invoice InvoiceMapper(IRDBDataReader reader)
        {
            Entities.Invoice invoice = new Entities.Invoice
            {
                Details = Vanrise.Common.Serializer.Deserialize(reader.GetString("Details")),
                FromDate = reader.GetDateTimeWithNullHandling("FromDate"),
                InvoiceId = reader.GetLong("ID"),
                InvoiceTypeId = reader.GetGuid("InvoiceTypeID"),
                IssueDate = reader.GetDateTimeWithNullHandling("IssueDate"),
                PartnerId = reader.GetString("PartnerID"),
                SerialNumber = reader.GetString("SerialNumber"),
                ToDate = reader.GetDateTimeWithNullHandling("ToDate"),
                PaidDate = reader.GetNullableDateTime("PaidDate"),
                DueDate = reader.GetDateTimeWithNullHandling("DueDate"),
                UserId = reader.GetIntWithNullHandling("UserId"),
                CreatedTime = reader.GetDateTimeWithNullHandling("CreatedTime"),
                LockDate = reader.GetNullableDateTime("LockDate"),
                Note = reader.GetString("Notes"),
                SourceId = reader.GetString("SourceId"),
                IsAutomatic = reader.GetBooleanWithNullHandling("IsAutomatic"),
                Settings = Vanrise.Common.Serializer.Deserialize<InvoiceSettings>(reader.GetString("Settings")),
                InvoiceSettingId = reader.GetGuidWithNullHandling("InvoiceSettingID"),
                SentDate = reader.GetNullableDateTime("SentDate"),
                SettlementInvoiceId = reader.GetNullableLong("SettlementInvoiceId"),
                SplitInvoiceGroupId = reader.GetNullableGuid("SplitInvoiceGroupId"),
                ApprovedBy = reader.GetNullableInt("ApprovedBy"),
                ApprovedTime = reader.GetNullableDateTime("ApprovedTime"),
                NeedApproval = reader.GetNullableBoolean("NeedApproval"),
            };
            return invoice;
        }

        public InvoiceByPartnerInfo InvoiceByPartnerInfoMapper(IRDBDataReader reader)
        {
            return new InvoiceByPartnerInfo
            {
                InvoiceTypeId = reader.GetGuidWithNullHandling("InvoiceTypeID"),
                IssueDate = reader.GetDateTimeWithNullHandling("IssueDate"),
                PartnerId = reader.GetString("PartnerID"),
                ToDate = reader.GetDateTimeWithNullHandling("ToDate"),
                DueDate = reader.GetDateTimeWithNullHandling("DueDate"),
            };
        }
        public VRPopulatedPeriod VRPopulatedPeriodMapper(IRDBDataReader reader)
        {
            return new VRPopulatedPeriod
            {
                FromDate = reader.GetNullableDateTime("FromDate"),
                ToDate = reader.GetNullableDateTime("ToDate"),
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
            where.EqualsCondition("InvoiceTypeID").Value(invoiceTypeId);
            if (partnerIds != null && partnerIds.Count() > 0)
                where.ListCondition("PartnerID", RDBListConditionOperator.IN, partnerIds);
            where.GreaterThanCondition("FromDate").Value(fromDate);
            where.LessThanCondition("ToDate").Value(toDate);
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
            where.EqualsCondition("InvoiceTypeID").Value(query.InvoiceTypeId);
            if (query.PartnerIds != null && query.PartnerIds.Count > 0)
                where.ListCondition("PartnerID", RDBListConditionOperator.IN, query.PartnerIds);
            if (query.PartnerPrefix != null)
                where.StartsWithCondition("PartnerID", query.PartnerPrefix);
            where.GreaterOrEqualCondition("FromDate").Value(query.FromTime);
            if (query.ToTime.HasValue)
                where.LessOrEqualCondition("ToDate").Value(query.ToTime.Value);
            if (query.IssueDate.HasValue)
                where.EqualsCondition("IssueDate").Value(query.IssueDate.Value);
            AddConditionInvoiceNotDeleted(where);
            if(query.IsPaid.HasValue)
            {
                if (query.IsPaid.Value)
                    where.NotNullCondition("PaidDate");
                else
                    where.NullCondition("PaidDate");
            }
            if(query.IssueDate.HasValue)
            {
                if (query.IsSent.Value)
                    where.NotNullCondition("SentDate");
                else
                    where.NullCondition("SentDate");
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
                    selectColumns.Column("InvoiceTypeID");
                    selectColumns.Column("ID", "InvoiceId");
                    selectColumns.Expression("InvoiceBulkActionIdentifier").Value(query.InvoiceBulkActionIdentifier.Value);
                }
            }


            var selectFromTempQuery = queryContext.AddSelectQuery();
            selectFromTempQuery.From(tempTableQuery, "tmp");
            selectFromTempQuery.SelectColumns().AllTableColumns("tmp");

            var invoices = queryContext.GetItems(InvoiceMapper);

            if(query.IsSelectAll.HasValue && query.InvoiceBulkActionIdentifier.HasValue)
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
            where.EqualsCondition("InvoiceTypeID").Value(InvoiceTypeId);
            AddConditionInvoiceNotDeleted(where);
            if (partnerId != null)
                where.EqualsCondition("PartnerID").Value(partnerId);
            if (fromDate.HasValue)
                where.GreaterOrEqualCondition("IssueDate").Value(fromDate.Value);
            if (toDate.HasValue)
                where.LessOrEqualCondition("IssueDate").Value(toDate.Value);

            return queryContext.ExecuteScalar().IntValue;
        }

        public bool SaveInvoices(List<GenerateInvoiceInputToSave> generateInvoicesInputToSave, out List<long> insertedInvoiceIds)
        {
            BillingTransactionDataManager billingTransactionDataManager = new BillingTransactionDataManager();
            insertedInvoiceIds = null;
            if (generateInvoicesInputToSave != null)
            {
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
                    insertQuery.GenerateIdAndAssignToParameter("ID");
                    insertQuery.Column("UserId").Value(generateInvoiceInputToSave.Invoice.UserId);
                    insertQuery.Column("PartnerID").Value(generateInvoiceInputToSave.Invoice.PartnerId);
                    if (splitInvoiceGroupId.HasValue)
                        insertQuery.Column("SplitInvoiceGroupId").Value(splitInvoiceGroupId.Value);
                    insertQuery.Column("InvoiceSettingID").Value(generateInvoiceInputToSave.Invoice.InvoiceSettingId);
                    insertQuery.Column("SerialNumber").Value(generateInvoiceInputToSave.Invoice.SerialNumber);
                    insertQuery.Column("FromDate").Value(generateInvoiceInputToSave.Invoice.FromDate);
                    insertQuery.Column("ToDate").Value(generateInvoiceInputToSave.Invoice.ToDate);
                    insertQuery.Column("IssueDate").Value(generateInvoiceInputToSave.Invoice.IssueDate);
                    insertQuery.Column("DueDate").Value(generateInvoiceInputToSave.Invoice.DueDate);
                    insertQuery.Column("Details").Value(serializedDetails);
                    insertQuery.Column("Notes").Value(generateInvoiceInputToSave.Invoice.Note);
                    insertQuery.Column("SourceId").Value(generateInvoiceInputToSave.Invoice.SourceId);
                    insertQuery.Column("IsDraft").Value(true);
                    insertQuery.Column("IsAutomatic").Value(generateInvoiceInputToSave.Invoice.IsAutomatic);
                    insertQuery.Column("Settings").Value(serializedSettings);
                    if (generateInvoiceInputToSave.Invoice.SentDate.HasValue)
                        insertQuery.Column("SentDate").Value(generateInvoiceInputToSave.Invoice.SentDate.Value);
                    if (generateInvoiceInputToSave.Invoice.NeedApproval.HasValue)
                        insertQuery.Column("NeedApproval").Value(generateInvoiceInputToSave.Invoice.NeedApproval.Value);
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
                        updateQuery.Column("SettlementInvoiceId").Value(generateInvoiceInputToSave.Invoice.InvoiceId);
                        updateQuery.Where().ListCondition("ID", RDBListConditionOperator.IN, generateInvoiceInputToSave.InvoiceToSettleIds);

                        if (generateInvoiceInputToSave.InvoiceIdToDelete.HasValue)
                        {
                            var updateQuery2 = queryContext2.AddUpdateQuery();
                            updateQuery2.FromTable(TABLE_NAME);
                            updateQuery2.Column("SettlementInvoiceId").Null();
                            updateQuery2.Where().EqualsCondition("SettlementInvoiceId").Value(generateInvoiceInputToSave.InvoiceIdToDelete.Value);
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
                        updateQuery.Column("IsDeleted").Value(true);
                        updateQuery.Where().EqualsCondition("ID").Value(generateInvoiceInputToSave.InvoiceIdToDelete.Value);

                        billingTransactionDataManager.AddQuerySetInvoiceBillingTransactionsDeleted(queryContext2, generateInvoiceInputToSave.InvoiceIdToDelete.Value);
                    }

                    var updateInvoiceQuery = queryContext2.AddUpdateQuery();
                    updateInvoiceQuery.FromTable(TABLE_NAME);
                    updateInvoiceQuery.Column("IsDraft").Value(false);
                    updateInvoiceQuery.Where().EqualsCondition("ID").Value(generateInvoiceInputToSave.Invoice.InvoiceId);
                }

                queryContext2.ExecuteNonQuery(true);
            }
            return true;
        }

        public List<Entities.Invoice> GetInvoices(List<long> invoiceIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv");
            selectQuery.SelectColumns().AllTableColumns("inv");

            selectQuery.Where().ListCondition("ID", RDBListConditionOperator.IN, invoiceIds);

            return queryContext.GetItems(InvoiceMapper);
        }

        public bool CheckInvoiceOverlaping(Guid invoiceTypeId, string partnerId, DateTime fromDate, DateTime toDate, long? invoiceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());            
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv", 1);
            selectQuery.SelectColumns().Column("ID");

            var where = selectQuery.Where();
            where.EqualsCondition("InvoiceTypeID").Value(invoiceTypeId);
            where.EqualsCondition("PartnerID", partnerId);
            where.GreaterOrEqualCondition("ToDate").Value(fromDate);
            where.LessOrEqualCondition("FromDate").Value(toDate);
            AddConditionInvoiceNotDeleted(where);
            if (invoiceId.HasValue)
                where.NotEqualsCondition("ID").Value(invoiceId.Value);

            return queryContext.ExecuteScalar().NullableLongValue.HasValue;
        }

        public bool SetInvoicePaid(long invoiceId, DateTime? paidDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            if (paidDate.HasValue)
                updateQuery.Column("PaidDate").Value(paidDate.Value);
            else
                updateQuery.Column("PaidDate").Null();
            updateQuery.Where().EqualsCondition("ID").Value(invoiceId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool SetInvoiceLocked(long invoiceId, DateTime? lockedDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            if (lockedDate.HasValue)
                updateQuery.Column("LockDate").Value(lockedDate.Value);
            else
                updateQuery.Column("LockDate").Null();
            updateQuery.Where().EqualsCondition("ID").Value(invoiceId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdateInvoiceNote(long invoiceId, string invoiceNote)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column("Notes").Value(invoiceNote);
            updateQuery.Where().EqualsCondition("ID").Value(invoiceId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public void LoadInvoicesAfterImportedId(Guid invoiceTypeId, long lastImportedId, Action<Entities.Invoice> onInvoiceReady)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv");
            selectQuery.SelectColumns().AllTableColumns("inv");

            var where = selectQuery.Where();
            where.EqualsCondition("InvoiceTypeID").Value(invoiceTypeId);
            AddConditionInvoiceNotDeleted(where);
            where.GreaterThanCondition("ID").Value(lastImportedId);

            selectQuery.Sort().ByColumn("ID", RDBSortDirection.ASC);

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
            tempTableQuery.AddColumn("InvoiceTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            tempTableQuery.AddColumn("PartnerID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });

            if(partnerInvoiceTypes != null)
            {
                foreach(var partner in partnerInvoiceTypes)
                {
                    var insertQuery = queryContext.AddInsertQuery();
                    insertQuery.IntoTable(tempTableQuery);
                    insertQuery.Column("InvoiceTypeID").Value(partner.InvoiceTypeId);
                    insertQuery.Column("PartnerID").Value(partner.PartnerId);
                }
            }

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv");
            selectQuery.SelectColumns().AllTableColumns("inv");

            var joinTempCondition = selectQuery.Join().Join(tempTableQuery, "partners").On();
            joinTempCondition.EqualsCondition("inv", "InvoiceTypeID", "partners", "InvoiceTypeID");
            joinTempCondition.EqualsCondition("inv", "PartnerID", "partners", "PartnerID");

            var where = selectQuery.Where();
            where.NullCondition().Column("PaidDate");
            where.LessOrEqualCondition("DueDate").DateNow();
            AddConditionInvoiceNotDeleted(where);

            return queryContext.GetItems(InvoiceMapper);
        }

        public IEnumerable<InvoiceByPartnerInfo> GetLastInvoicesByPartners(IEnumerable<PartnerInvoiceType> partnerInvoiceTypes)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn("InvoiceTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            tempTableQuery.AddColumn("PartnerID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });

            if (partnerInvoiceTypes != null)
            {
                foreach (var partner in partnerInvoiceTypes)
                {
                    var insertQuery = queryContext.AddInsertQuery();
                    insertQuery.IntoTable(tempTableQuery);
                    insertQuery.Column("InvoiceTypeID").Value(partner.InvoiceTypeId);
                    insertQuery.Column("PartnerID").Value(partner.PartnerId);
                }
            }

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv");
            selectQuery.SelectColumns().Columns("InvoiceTypeID", "PartnerID", "IssueDate", "ToDate", "DueDate");

            var joinTempCondition = selectQuery.Join().Join(tempTableQuery, "partners").On();
            joinTempCondition.EqualsCondition("inv", "InvoiceTypeID", "partners", "InvoiceTypeID");
            joinTempCondition.EqualsCondition("inv", "PartnerID", "partners", "PartnerID");

            var where = selectQuery.Where();
            where.LessOrEqualCondition("DueDate").DateNow();
            AddConditionInvoiceNotDeleted(where);

            return queryContext.GetItems(InvoiceByPartnerInfoMapper);
        }

        public bool Update(Entities.Invoice invoice)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();

            updateQuery.Column("InvoiceTypeID").Value(invoice.InvoiceTypeId);
            updateQuery.Column("PartnerID").Value(invoice.InvoiceTypeId);
            updateQuery.Column("SerialNumber").Value(invoice.SerialNumber);
            updateQuery.Column("FromDate").Value(invoice.FromDate);
            updateQuery.Column("ToDate").Value(invoice.ToDate);
            updateQuery.Column("IssueDate").Value(invoice.IssueDate);
            updateQuery.Column("DueDate").Value(invoice.DueDate);
            updateQuery.Column("Details").Value(Vanrise.Common.Serializer.Serialize(invoice.Details));
            if (invoice.PaidDate.HasValue)
                updateQuery.Column("PaidDate").Value(invoice.PaidDate.Value);
            else
                updateQuery.Column("PaidDate").Null();
            if (invoice.LockDate.HasValue)
                updateQuery.Column("LockDate").Value(invoice.LockDate.Value);
            else
                updateQuery.Column("LockDate").Null();
            updateQuery.Column("Notes").Value(invoice.Note);
            updateQuery.Column("SourceId").Value(invoice.SourceId);
            updateQuery.Column("InvoiceSettingID").Value(invoice.InvoiceSettingId);
            if (invoice.SentDate.HasValue)
                updateQuery.Column("SentDate").Value(invoice.SentDate.Value);
            else
                updateQuery.Column("SentDate").Null();

            updateQuery.Where().EqualsCondition("ID").Value(invoice.InvoiceId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public Entities.Invoice GetInvoiceBySourceId(Guid invoiceTypeId, string sourceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv");
            selectQuery.SelectColumns().AllTableColumns("inv");

            var where = selectQuery.Where();
            where.EqualsCondition("InvoiceTypeID").Value(invoiceTypeId);
            where.EqualsCondition("SourceId").Value(sourceId);
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
            where.EqualsCondition("InvoiceTypeID").Value(invoiceTypeId);
            where.EqualsCondition("PartnerID").Value(partnerId);
            AddConditionInvoiceNotDeleted(where);

            selectQuery.Sort().ByColumn("ID", RDBSortDirection.DESC);

            return queryContext.GetItem(InvoiceMapper);
        }

        public bool UpdateInvoicePaidDateBySourceId(Guid invoiceTypeId, string sourceId, DateTime paidDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column("PaidDate").Value(paidDate);

            var where = updateQuery.Where();
            where.EqualsCondition("InvoiceTypeID").Value(invoiceTypeId);
            where.EqualsCondition("SourceId").Value(sourceId);
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
            where.EqualsCondition("InvoiceTypeID").Value(invoiceTypeId);
            where.EqualsCondition("PartnerID").Value(partnerId);
            AddConditionInvoiceNotDeleted(where);
            if (beforeDate.HasValue)
                where.LessThanCondition("CreatedTime").Value(beforeDate.Value);

            selectQuery.Sort().ByColumn("ID", RDBSortDirection.DESC);

            return queryContext.GetItems(InvoiceMapper);
        }

        public VRPopulatedPeriod GetInvoicesPopulatedPeriod(Guid invoiceTypeId, string partnerId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv");

            var aggregates = selectQuery.SelectAggregates();
            aggregates.Aggregate(RDBNonCountAggregateType.MIN, "FromDate");
            aggregates.Aggregate(RDBNonCountAggregateType.MAX, "ToDate");

            var where = selectQuery.Where();
            where.EqualsCondition("InvoiceTypeID").Value(invoiceTypeId);
            where.EqualsCondition("PartnerID").Value(partnerId);
            AddConditionInvoiceNotDeleted(where);

            return queryContext.GetItem(VRPopulatedPeriodMapper);
        }

        public bool CheckPartnerIfHasInvoices(Guid invoiceTypeId, string partnerId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv", 1);
            selectQuery.SelectColumns().Column("ID");

            var where = selectQuery.Where();
            where.EqualsCondition("InvoiceTypeID").Value(invoiceTypeId);
            where.EqualsCondition("PartnerID").Value(partnerId);

            return queryContext.ExecuteScalar().NullableLongValue.HasValue;
        }

        public List<Entities.Invoice> GetInvoicesBySerialNumbers(Guid invoiceTypeId, IEnumerable<string> serialNumbers)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "inv");
            selectQuery.SelectColumns().AllTableColumns("inv");

            var where = selectQuery.Where();
            where.EqualsCondition("InvoiceTypeID").Value(invoiceTypeId);
            AddConditionInvoiceNotDeleted(where);
            where.ListCondition("SerialNumber", RDBListConditionOperator.IN, serialNumbers);

            return queryContext.GetItems(InvoiceMapper);
        }

        public bool UpdateInvoicePaidDateById(Guid invoiceTypeId, long invoiceId, DateTime paidDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column("PaidDate").Value(paidDate);

            var where = updateQuery.Where();
            where.EqualsCondition("InvoiceTypeID").Value(invoiceTypeId);
            where.EqualsCondition("ID").Value(invoiceId);

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
            updateQuery.Column("Settings").Value(serializedSettings);

            var where = updateQuery.Where();
            where.EqualsCondition("ID").Value(invoiceId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool SetInvoiceSentDate(long invoiceId, DateTime? sentDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            if (sentDate.HasValue)
                updateQuery.Column("SentDate").Value(sentDate.Value);
            else
                updateQuery.Column("SentDate").Null();
            updateQuery.Where().EqualsCondition("ID").Value(invoiceId);
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
            updateQuery.Column("IsDeleted").Value(true);
            updateQuery.Where().EqualsCondition("ID").Value(invoiceId);

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
            where.EqualsCondition("InvoiceTypeID").Value(invoiceTypeId);
            if (from.HasValue)
                where.GreaterOrEqualCondition("CreatedTime").Value(from.Value);
            if (to.HasValue)
                where.LessOrEqualCondition("CreatedTime").Value(to.Value);
            if (filterGroup != null)
                new RecordFilterRDBBuilder((fieldName, expressionContext) => expressionContext.Column("inv", GetColumnNameFromFieldName(fieldName))).RecordFilterGroupCondition(where, filterGroup);

            if (orderDirection.HasValue)
                selectQuery.Sort().ByColumn("ID", orderDirection.Value == OrderDirection.Ascending ? RDBSortDirection.ASC : RDBSortDirection.DESC);

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

        #endregion

        private void AddConditionInvoiceNotDeleted(RDBConditionContext conditionContext)
        {
            conditionContext.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted").Value(false);
            conditionContext.ConditionIfColumnNotNull("IsDraft").EqualsCondition("IsDraft").Value(false);
        }

        internal void AddJoinInvoiceToBulkActionDraft(RDBJoinContext joinContext, string bulkActionDraftAlias, string invoiceAlias)
        {
            var joinCondition = joinContext.Join(TABLE_NAME, invoiceAlias).On();
            joinCondition.EqualsCondition(invoiceAlias, "InvoiceTypeID").Column(bulkActionDraftAlias, "InvoiceTypeID");
            joinCondition.EqualsCondition(invoiceAlias, "ID").Column(bulkActionDraftAlias, "InvoiceId");
            AddConditionInvoiceNotDeleted(joinCondition);
        }
    }
}

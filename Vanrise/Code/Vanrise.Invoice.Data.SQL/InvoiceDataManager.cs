using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;
using Vanrise.Common;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data.SQL
{
    public class InvoiceDataManager : BaseSQLDataManager, IInvoiceDataManager
    {
        #region Fields / Constructors

        const string PartnerInvoiceType_TABLENAME = "PartnerInvoiceTypeTable";
        public InvoiceDataManager()
            : base(GetConnectionStringName("InvoiceDBConnStringKey", "InvoiceDBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public bool SetInvoicePaid(long invoiceId, DateTime? paidDate)
        {
            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_Invoice_UpdateInvoicePaid", invoiceId, paidDate);
            return (affectedRows > -1);
        }
        public bool UpdateInvoicePaidDateBySourceId(Guid invoiceTypeId, string sourceId, DateTime paidDate)
        {
            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_Invoice_UpdateInvoicePaidBySourceId", invoiceTypeId, sourceId, paidDate);
            return (affectedRows > -1);
        }
        public bool UpdateInvoicePaidDateById(Guid invoiceTypeId, long invoiceId, DateTime paidDate)
        {
            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_Invoice_UpdateInvoicePaidById", invoiceTypeId, invoiceId, paidDate);
            return (affectedRows > -1);
        }
        public bool UpdateInvoiceNote(long invoiceId, string invoiceNote)
        {
            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_Invoice_UpdateInvoiceNote", invoiceId, invoiceNote);
            return (affectedRows > -1);
        }
        public bool SetInvoiceLocked(long invoiceId, DateTime? lockedDate)
        {
            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_Invoice_UpdateInvoiceLock", invoiceId, lockedDate);
            return (affectedRows > -1);
        }
        public bool SetInvoiceSentDate(long invoiceId, DateTime? sentDate)
        {
            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_Invoice_UpdateInvoiceSentDate", invoiceId, sentDate);
            return (affectedRows > -1);
        }
        public Entities.Invoice GetInvoiceBySourceId(Guid invoiceTypeId, string sourceId)
        {
            return GetItemSP("VR_Invoice.sp_Invoice_GetBySourceId", InvoiceMapper, invoiceTypeId, sourceId);
        }
        public bool CheckInvoiceOverlaping(Guid invoiceTypeId, string partnerId, DateTime fromDate, DateTime toDate, long? invoiceId)
        {
            int numberOfRecords = GetItemSP("VR_Invoice.sp_Invoice_CheckOverlaping", (reader) => { return (int)reader["CountNb"]; }, invoiceTypeId, partnerId, fromDate, toDate, invoiceId);
            return (numberOfRecords > 0);
        }
        public IEnumerable<Entities.Invoice> GetFilteredInvoices(DataRetrievalInput<InvoiceQuery> input)
        {
            string partnerIds = null;
            if (input.Query.PartnerIds != null && input.Query.PartnerIds.Count() > 0)
                partnerIds = string.Join<string>(",", input.Query.PartnerIds);

            return GetItemsSP("VR_Invoice.sp_Invoice_GetFiltered", InvoiceMapper, input.Query.InvoiceTypeId, partnerIds, input.Query.PartnerPrefix, input.Query.FromTime, input.Query.ToTime, input.Query.IssueDate, input.Query.EffectiveDate, input.Query.IsEffectiveInFuture, input.Query.Status, input.Query.IsSelectAll, input.Query.InvoiceBulkActionIdentifier, input.Query.IsSent, input.Query.IsPaid);
        }

        public List<Entities.Invoice> GetPartnerInvoicesByDate(Guid invoiceTypeId, IEnumerable<string> partnerIds, DateTime fromDate, DateTime toDate)
        {

            string partnerIdsAsString = null;
            if (partnerIds != null && partnerIds.Count() > 0)
                partnerIdsAsString = string.Join<string>(",", partnerIds);

            return GetItemsSP("VR_Invoice.sp_Invoice_GetByDate", InvoiceMapper, invoiceTypeId, partnerIdsAsString, fromDate, toDate);
        }

        public IEnumerable<InvoiceByPartnerInfo> GetLastInvoicesByPartners(IEnumerable<PartnerInvoiceType> partnerInvoiceTypes)
        {
            DataTable partnerInvoiceTypeTable = GetPartnerInvoiceTypeTable();
            foreach (var partnerInvoiceType in partnerInvoiceTypes)
            {
                DataRow dr = partnerInvoiceTypeTable.NewRow();
                FillPartnerInvoiceTypeRow(dr, partnerInvoiceType.InvoiceTypeId, partnerInvoiceType.PartnerId);
                partnerInvoiceTypeTable.Rows.Add(dr);
            }
            partnerInvoiceTypeTable.EndLoadData();
            if (partnerInvoiceTypeTable.Rows.Count > 0)
                return GetItemsSPCmd("[VR_Invoice].[sp_Invoice_GetLastByPartners]", InvoiceByPartnerInfoMapper,
                          (cmd) =>
                          {
                              var dtPrm = new System.Data.SqlClient.SqlParameter("@PartnerInvoiceTypeTable", SqlDbType.Structured);
                              dtPrm.Value = partnerInvoiceTypeTable;
                              cmd.Parameters.Add(dtPrm);
                          });
            return null;
        }
        public IEnumerable<Entities.Invoice> GetUnPaidPartnerInvoices(IEnumerable<PartnerInvoiceType> partnerInvoiceTypes)
        {
            DataTable partnerInvoiceTypeTable = GetPartnerInvoiceTypeTable();
            foreach (var partnerInvoiceType in partnerInvoiceTypes)
            {
                DataRow dr = partnerInvoiceTypeTable.NewRow();
                FillPartnerInvoiceTypeRow(dr, partnerInvoiceType.InvoiceTypeId, partnerInvoiceType.PartnerId);
                partnerInvoiceTypeTable.Rows.Add(dr);
            }
            partnerInvoiceTypeTable.EndLoadData();
            if (partnerInvoiceTypeTable.Rows.Count > 0)
                return GetItemsSPCmd("[VR_Invoice].[sp_Invoice_GetUnpaidByPartner]", InvoiceMapper,
                          (cmd) =>
                          {
                              var dtPrm = new System.Data.SqlClient.SqlParameter("@PartnerInvoiceTypeTable", SqlDbType.Structured);
                              dtPrm.Value = partnerInvoiceTypeTable;
                              cmd.Parameters.Add(dtPrm);
                          });
            return null;
        }
        public bool Update(Entities.Invoice invoice)
        {
            return ExecuteNonQuerySP("[VR_Invoice].[sp_Invoice_Update]", invoice.InvoiceId,
                                                                         invoice.InvoiceTypeId,
                                                                         invoice.PartnerId,
                                                                         invoice.SerialNumber,
                                                                         invoice.FromDate,
                                                                         invoice.ToDate,
                                                                         invoice.IssueDate,
                                                                         invoice.DueDate,
                                                                         Vanrise.Common.Serializer.Serialize(invoice.Details),
                                                                         invoice.PaidDate,
                                                                         invoice.LockDate,
                                                                         invoice.Note,
                                                                         invoice.SourceId,
                                                                         invoice.InvoiceSettingId,
                                                                         invoice.SentDate) > 0;
        }
        public int GetInvoiceCount(Guid InvoiceTypeId, string partnerId, DateTime? fromDate, DateTime? toDate)
        {
            return GetItemSP("VR_Invoice.sp_Invoice_GetInvoiceCount", (reader) =>
            {
                return GetReaderValue<int>(reader, "Counter");
            }, InvoiceTypeId, partnerId, fromDate, toDate);
        }

        public bool SaveInvoices(List<GenerateInvoiceInputToSave> generateInvoicesInputToSave, out List<long> insertedInvoiceIds)
        {
            insertedInvoiceIds = null;
            if (generateInvoicesInputToSave != null)
            {
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

                    object invoiceId;
                    int affectedRows = ExecuteNonQuerySP
                    (
                        "[VR_Invoice].[sp_Invoice_Save]",
                        out invoiceId,
                        generateInvoiceInputToSave.Invoice.UserId,
                        generateInvoiceInputToSave.Invoice.InvoiceTypeId,
                        generateInvoiceInputToSave.Invoice.PartnerId,
                        generateInvoiceInputToSave.Invoice.SerialNumber,
                        generateInvoiceInputToSave.Invoice.FromDate,
                        generateInvoiceInputToSave.Invoice.ToDate,
                        generateInvoiceInputToSave.Invoice.IssueDate,
                        generateInvoiceInputToSave.Invoice.DueDate,
                        Vanrise.Common.Serializer.Serialize(generateInvoiceInputToSave.Invoice.Details),
                        generateInvoiceInputToSave.Invoice.Note,
                        generateInvoiceInputToSave.Invoice.SourceId,
                        true,
                        generateInvoiceInputToSave.Invoice.IsAutomatic,
                        serializedSettings,
                        generateInvoiceInputToSave.Invoice.InvoiceSettingId,
                        generateInvoiceInputToSave.Invoice.SentDate,
                        splitInvoiceGroupId,
                        generateInvoiceInputToSave.Invoice.NeedApproval
                    );
                    long insertedInvoiceId = Convert.ToInt64(invoiceId);

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

                var transactionOptions = new TransactionOptions
                {
                    IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                    Timeout = TransactionManager.DefaultTimeout
                };

                using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                {
                    var transactionDataManager = new Vanrise.AccountBalance.Data.SQL.BillingTransactionDataManager();

                    foreach (var generateInvoiceInputToSave in generateInvoicesInputToSave)
                    {

                        if (generateInvoiceInputToSave.InvoiceToSettleIds != null && generateInvoiceInputToSave.InvoiceToSettleIds.Count > 0)
                        {
                            UpdateInvoiceSettleIds(generateInvoiceInputToSave.Invoice.InvoiceId, generateInvoiceInputToSave.InvoiceToSettleIds);
                            if (generateInvoiceInputToSave.InvoiceIdToDelete.HasValue)
                            {
                                ClearSettlementInvoiceIds(generateInvoiceInputToSave.InvoiceIdToDelete.Value);
                            }
                        }
                        if (generateInvoiceInputToSave.MappedTransactions != null && generateInvoiceInputToSave.MappedTransactions.Count() > 0)
                            InsertBillingTransactions(generateInvoiceInputToSave.MappedTransactions, generateInvoiceInputToSave.Invoice.InvoiceId, transactionDataManager);

                        if (generateInvoiceInputToSave.InvoiceIdToDelete.HasValue)
                        {

                            DeleteInvoice(generateInvoiceInputToSave.InvoiceIdToDelete.Value);

                            transactionDataManager.SetBillingTransactionsAsDeleted(generateInvoiceInputToSave.InvoiceIdToDelete.Value);
                        }
                        SetDraft(generateInvoiceInputToSave.Invoice.InvoiceId, false);
                    }
                    transactionScope.Complete();
                }
            }
            return true;
        }
        public bool ClearSettlementInvoiceIds(long settlementInvoiceId)
        {
            int affectedRows = ExecuteNonQuerySP("[VR_Invoice].[sp_Invoice_ClearSettlementId]", settlementInvoiceId);
            return (affectedRows > -1);
        }
        public bool UpdateInvoiceSettleIds(long invoiceId, List<long> invoiceToSettleIds)
        {
            string invoiceToSettleIdsAsString = null;
            if (invoiceToSettleIds != null)
            {
                invoiceToSettleIdsAsString = string.Join<long>(",", invoiceToSettleIds);
            }
            int affectedRows = ExecuteNonQuerySP("[VR_Invoice].[sp_Invoice_UpdateSettleIds]", invoiceId, invoiceToSettleIdsAsString);
            return (affectedRows > -1);
        }
        public bool SetDraft(long invoiceId, bool isDraft)
        {
            int affectedRows = ExecuteNonQuerySP("[VR_Invoice].[sp_Invoice_SetDraft]", invoiceId, isDraft);
            return (affectedRows > -1);
        }
        public bool DeleteInvoice(long deletedInvoiceId)
        {
            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_Invoice_Delete", deletedInvoiceId);
            return affectedRows > 0;
        }
        public bool DeleteGeneratedInvoice(long invoiceId, Guid invoiceTypeId, string partnerId, DateTime fromDate, DateTime toDate)
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                Timeout = TransactionManager.DefaultTimeout
            };
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                var transactionDataManager = new Vanrise.AccountBalance.Data.SQL.BillingTransactionDataManager();
                DeleteInvoice(invoiceId);
                BillingPeriodInfoDataManager billingPeriodInfoDataManager = new SQL.BillingPeriodInfoDataManager();

                var billingPeriodInfo = billingPeriodInfoDataManager.GetBillingPeriodInfoById(partnerId, invoiceTypeId);
                var nextPeriodStart = toDate.AddDays(1);
                if (billingPeriodInfo.NextPeriodStart.Date == nextPeriodStart.Date)
                {
                    billingPeriodInfoDataManager.InsertOrUpdateBillingPeriodInfo(new BillingPeriodInfo
                    {
                        InvoiceTypeId = invoiceTypeId,
                        NextPeriodStart = fromDate,
                        PartnerId = partnerId
                    });
                }
                transactionDataManager.SetBillingTransactionsAsDeleted(invoiceId);
                transactionScope.Complete();
            }
            return true;
        }
        public void LoadInvoicesAfterImportedId(Guid invoiceTypeId, long lastImportedId, Action<Entities.Invoice> onInvoiceReady)
        {
            ExecuteReaderSP("VR_Invoice.sp_Invoice_GetAfterImportedID", (reader) =>
            {
                while (reader.Read())
                {
                    onInvoiceReady(InvoiceMapper(reader));
                }
            }, invoiceTypeId, lastImportedId);
        }
        public bool AreInvoicesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("VR_Invoice.Invoice", ref updateHandle);
        }

        public bool UpdateInvoiceSettings(long invoiceId, InvoiceSettings invoiceSettings)
        {
            string serializedSettings = null;
            if (invoiceSettings != null)
            {
                serializedSettings = Vanrise.Common.Serializer.Serialize(invoiceSettings);
            }
            return ExecuteNonQuerySP("[VR_Invoice].[sp_Invoice_UpdateSettings]", invoiceId, serializedSettings) > 0;
        }
        public Entities.Invoice GetLastInvoice(Guid invoiceTypeId, string partnerId)
        {
            return GetItemSP("VR_Invoice.sp_Invoice_GetLast", InvoiceMapper, invoiceTypeId, partnerId);
        }

        public IEnumerable<Entities.Invoice> GetLasInvoices(Guid invoiceTypeId, string partnerId, DateTime? beforeDate, int lastInvoices)
        {
            return GetItemsSP("VR_Invoice.sp_Invoice_GetLastInvoices", InvoiceMapper, invoiceTypeId, partnerId, beforeDate, lastInvoices);
        }
        public VRPopulatedPeriod GetInvoicesPopulatedPeriod(Guid invoiceTypeId, string partnerId)
        {
            return GetItemSP("VR_Invoice.sp_Invoice_GetPopulatedPeriod", VRPopulatedPeriodMapper, invoiceTypeId, partnerId);
        }
        public bool CheckPartnerIfHasInvoices(Guid invoiceTypeId, string partnerId)
        {
            return GetItemSP("VR_Invoice.sp_Invoice_CheckIfHasInvoices", (reader) => { return (bool)reader["HasInvoices"]; }, invoiceTypeId, partnerId);
        }
        public List<Entities.Invoice> GetInvoicesBySerialNumbers(Guid invoiceTypeId, IEnumerable<string> serialNumbers)
        {
            return GetItemsSP("[VR_Invoice].[sp_Invoice_GetBySerialNumbers]", InvoiceMapper, invoiceTypeId, string.Join(",", serialNumbers));
        }

        public List<Entities.Invoice> GetInvoices(List<long> invoiceIds)
        {
            string invoiceIdsAsString = null;
            if (invoiceIds != null && invoiceIds.Count() > 0)
                invoiceIdsAsString = string.Join<long>(",", invoiceIds);

            return GetItemsSP("VR_Invoice.sp_Invoice_Get", InvoiceMapper, invoiceIdsAsString);
        }
        public bool ApproveInvoice(long invoiceId, DateTime? ApprovedDate, int? ApprovedBy)
        {
            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_Invoice_UpdateInvoiceApproved", invoiceId, ApprovedDate, ApprovedBy);
            return (affectedRows > -1);
        }

        #endregion

        #region Private Methods

        private void FillPartnerInvoiceTypeRow(DataRow dr, Guid invoiceTypeId, string partnerId)
        {
            dr["InvoiceTypeId"] = invoiceTypeId;
            dr["PartnerId"] = partnerId;
        }

        private DataTable GetPartnerInvoiceTypeTable()
        {
            DataTable dt = new DataTable(PartnerInvoiceType_TABLENAME);
            dt.Columns.Add("InvoiceTypeId", typeof(Guid));
            dt.Columns.Add("PartnerId", typeof(string));
            return dt;
        }

        private bool InsertBillingTransactions(IEnumerable<Vanrise.AccountBalance.Entities.BillingTransaction> billingTransactions, long invoiceId, Vanrise.AccountBalance.Data.SQL.BillingTransactionDataManager billingTransactionDataManager)
        {
            long transactionId;
            bool areAllInsertionsSuccessful = true;

            foreach (Vanrise.AccountBalance.Entities.BillingTransaction billingTransaction in billingTransactions)
            {
                bool isInsertionSuccessful = billingTransactionDataManager.Insert(billingTransaction, invoiceId, out transactionId);
                if (!isInsertionSuccessful)
                    areAllInsertionsSuccessful = false;
            }

            return areAllInsertionsSuccessful;
        }

        #endregion

        #region Mappers
        public Entities.Invoice InvoiceMapper(IDataReader reader)
        {
            Entities.Invoice invoice = new Entities.Invoice
            {
                Details = Vanrise.Common.Serializer.Deserialize(reader["Details"] as string),
                FromDate = GetReaderValue<DateTime>(reader, "FromDate"),
                InvoiceId = GetReaderValue<long>(reader, "ID"),
                InvoiceTypeId = GetReaderValue<Guid>(reader, "InvoiceTypeId"),
                IssueDate = GetReaderValue<DateTime>(reader, "IssueDate"),
                PartnerId = reader["PartnerId"] as string,
                SerialNumber = reader["SerialNumber"] as string,
                ToDate = GetReaderValue<DateTime>(reader, "ToDate"),
                PaidDate = GetReaderValue<DateTime?>(reader, "PaidDate"),
                DueDate = GetReaderValue<DateTime>(reader, "DueDate"),
                UserId = GetReaderValue<int>(reader, "UserId"),
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                LockDate = GetReaderValue<DateTime?>(reader, "LockDate"),
                Note = reader["Notes"] as string,
                SourceId = reader["SourceID"] as string,
                IsAutomatic = GetReaderValue<Boolean>(reader, "IsAutomatic"),
                Settings = Vanrise.Common.Serializer.Deserialize<InvoiceSettings>(reader["Settings"] as string),
                InvoiceSettingId = GetReaderValue<Guid>(reader, "InvoiceSettingId"),
                SentDate = GetReaderValue<DateTime?>(reader, "SentDate"),
                SettlementInvoiceId = GetReaderValue<long?>(reader, "SettlementInvoiceId"),
                SplitInvoiceGroupId = GetReaderValue<Guid?>(reader, "SplitInvoiceGroupId"),
                ApprovedBy = GetReaderValue<int?>(reader, "ApprovedBy"),
                ApprovedTime = GetReaderValue<DateTime?>(reader, "ApprovedTime"),
                NeedApproval = GetReaderValue<bool?>(reader, "NeedApproval"),
            };
            return invoice;
        }

        public InvoiceByPartnerInfo InvoiceByPartnerInfoMapper(IDataReader reader)
        {
            return new InvoiceByPartnerInfo
            {
                InvoiceTypeId = GetReaderValue<Guid>(reader, "InvoiceTypeId"),
                IssueDate = GetReaderValue<DateTime>(reader, "IssueDate"),
                PartnerId = reader["PartnerId"] as string,
                ToDate = GetReaderValue<DateTime>(reader, "ToDate"),
                DueDate = GetReaderValue<DateTime>(reader, "DueDate"),
            };
        }
        public VRPopulatedPeriod VRPopulatedPeriodMapper(IDataReader reader)
        {
            return new VRPopulatedPeriod
            {
                FromDate = GetReaderValue<DateTime?>(reader, "FromDate"),
                ToDate = GetReaderValue<DateTime?>(reader, "ToDate"),
            };
        }

        #endregion


        public void LoadInvoices(Guid invoiceTypeId, DateTime? fromTime, DateTime? toTime, RecordFilterGroup filterGroup, OrderDirection? orderDirection, Func<bool> shouldStop, Action<Entities.Invoice> onInvoiceLoaded)
        {
            int parameterIndex = 0;
            Dictionary<string, Object> parameterValues = new Dictionary<string, object>();
            Vanrise.GenericData.Data.SQL.RecordFilterSQLBuilder recordFilterSQLBuilder = new Vanrise.GenericData.Data.SQL.RecordFilterSQLBuilder(GetColumnNameFromFieldName);
            String recordFilter = recordFilterSQLBuilder.BuildRecordFilter(filterGroup, ref parameterIndex, parameterValues);
            string query = BuildQuery(recordFilter, orderDirection);

            ExecuteReaderText(query,
                (reader) =>
                {
                    while (reader.Read())
                    {
                        onInvoiceLoaded(InvoiceMapper(reader));
                        if (shouldStop())
                            break;
                    }
                }, (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@InvoiceTypeId", invoiceTypeId));
                    cmd.Parameters.Add(new SqlParameter("@FromTime", ToDBNullIfDefault(fromTime)));
                    cmd.Parameters.Add(new SqlParameter("@ToTime", ToDBNullIfDefault(toTime)));
                    foreach (var prm in parameterValues)
                    {
                        cmd.Parameters.Add(new SqlParameter(prm.Key, prm.Value));
                    }
                });
        }


        private string BuildQuery(string recordFilter, OrderDirection? orderDirection)
        {
            string queryRecordFilter = !string.IsNullOrEmpty(recordFilter) ? string.Format(" and {0} ", recordFilter) : string.Empty;
            string queryOrder = orderDirection.HasValue ? string.Format(" Order By ID {0} ", orderDirection == OrderDirection.Ascending ? "ASC" : "DESC") : string.Empty;

            StringBuilder str = new StringBuilder(string.Format(@"  select {0} from [VR_Invoice].[Invoice] WITH (NOLOCK)
                                                                    where [InvoiceTypeID] = @InvoiceTypeId 
                                                                    and (@FromTime is null or [CreatedTime] >= @FromTime) 
                                                                    and (@ToTime is null or [CreatedTime] <= @ToTime) 
                                                                    {1} 
                                                                    {2}",
                                                                    InvoiceSELECTCOLUMNS, queryRecordFilter, queryOrder));
            return str.ToString();
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

        const string InvoiceSELECTCOLUMNS = @" ID,InvoiceTypeID,PartnerID,SerialNumber,FromDate,ToDate,SplitInvoiceGroupId,IssueDate,DueDate,Details,PaidDate,UserId,CreatedTime,LockDate,Notes,IsAutomatic, SourceId,Settings,InvoiceSettingID,SentDate,SettlementInvoiceId,ApprovedBy,ApprovedTime,NeedApproval ";
    }
}

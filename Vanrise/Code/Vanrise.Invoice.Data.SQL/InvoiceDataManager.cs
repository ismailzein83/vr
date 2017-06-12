﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Vanrise.Common;
using Vanrise.Data.SQL;
using Vanrise.Entities;
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
        public Entities.Invoice GetInvoice(long invoiceId)
        {
            return GetItemSP("VR_Invoice.sp_Invoice_Get", InvoiceMapper, invoiceId);
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

            return GetItemsSP("VR_Invoice.sp_Invoice_GetFiltered", InvoiceMapper, input.Query.InvoiceTypeId, partnerIds, input.Query.PartnerPrefix, input.Query.FromTime, input.Query.ToTime, input.Query.IssueDate, input.Query.EffectiveDate, input.Query.IsEffectiveInFuture, input.Query.Status);
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
                                                                         invoice.TimeZoneId,
                                                                         invoice.TimeZoneOffset,
                                                                         invoice.IssueDate,
                                                                         invoice.DueDate,
                                                                         Vanrise.Common.Serializer.Serialize(invoice.Details),
                                                                         invoice.PaidDate,
                                                                         invoice.LockDate,
                                                                         invoice.Note,
                                                                         invoice.SourceId) > 0;
        }
        public int GetInvoiceCount(Guid InvoiceTypeId, string partnerId, DateTime? fromDate, DateTime? toDate)
        {
            return GetItemSP("VR_Invoice.sp_Invoice_GetInvoiceCount", (reader) =>
            {
                return GetReaderValue<int>(reader, "Counter");
            }, InvoiceTypeId, partnerId, fromDate, toDate);
        }
        public bool SaveInvoices(List<GeneratedInvoiceItemSet> invoiceItemSets, Entities.Invoice invoiceEntity, long? invoiceIdToDelete, Dictionary<string, List<string>> itemSetNameStorageDic, IEnumerable<GeneratedInvoiceBillingTransaction> billingTransactions, out long insertedInvoiceId)
        {
            object invoiceId;

            int affectedRows = ExecuteNonQuerySP
            (
                "[VR_Invoice].[sp_Invoice_Save]",
                out invoiceId,
                invoiceEntity.UserId,
                invoiceEntity.InvoiceTypeId,
                invoiceEntity.PartnerId,
                invoiceEntity.SerialNumber,
                invoiceEntity.FromDate,
                invoiceEntity.ToDate,
                invoiceEntity.TimeZoneId,
                invoiceEntity.TimeZoneOffset,
                invoiceEntity.IssueDate,
                invoiceEntity.DueDate,
                Vanrise.Common.Serializer.Serialize(invoiceEntity.Details),
                invoiceEntity.Note,
                invoiceEntity.SourceId,
                true
            );

            insertedInvoiceId = Convert.ToInt64(invoiceId);

            if (itemSetNameStorageDic != null && itemSetNameStorageDic.Count > 0)
            {
                var remainingInvoiceItemSets = invoiceItemSets.FindAllRecords(x => !itemSetNameStorageDic.Values.Any(y => y.Contains(x.SetName)));
                if (remainingInvoiceItemSets != null)
                {
                    InvoiceItemDataManager dataManager = new InvoiceItemDataManager();
                    dataManager.SaveInvoiceItems(insertedInvoiceId, remainingInvoiceItemSets);
                }
                foreach (var item in itemSetNameStorageDic)
                {
                    InvoiceItemDataManager dataManager = new InvoiceItemDataManager();
                    dataManager.StorageConnectionStringKey = item.Key;
                    var invoiceItemSetsToSave = invoiceItemSets.FindAllRecords(x => item.Value.Contains(x.SetName));
                    dataManager.SaveInvoiceItems(insertedInvoiceId, invoiceItemSetsToSave);
                }
            }
            else
            {
                InvoiceItemDataManager dataManager = new InvoiceItemDataManager();
                dataManager.SaveInvoiceItems(insertedInvoiceId, invoiceItemSets);
            }

            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                Timeout = TransactionManager.DefaultTimeout
            };

            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                var transactionDataManager = new Vanrise.AccountBalance.Data.SQL.BillingTransactionDataManager();

                if (billingTransactions != null && billingTransactions.Count() > 0)
                    InsertBillingTransactions(billingTransactions, insertedInvoiceId, transactionDataManager);

                if (invoiceIdToDelete.HasValue)
                {
                    DeleteInvoice(invoiceIdToDelete.Value);
                    transactionDataManager.SetBillingTransactionsAsDeleted(invoiceIdToDelete.Value);
                }

                SetDraft(insertedInvoiceId, false);
                transactionScope.Complete();
            }

            return true;
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


        public Entities.Invoice GetLastInvoice(Guid invoiceTypeId, string partnerId)
        {
            return GetItemSP("VR_Invoice.sp_Invoice_GetLast", InvoiceMapper, invoiceTypeId, partnerId);
        }

        public IEnumerable<Entities.Invoice> GetLasInvoices(Guid invoiceTypeId, string partnerId, DateTime? beforeDate, int lastInvoices)
        {
            return GetItemsSP("VR_Invoice.sp_Invoice_GetLastInvoices", InvoiceMapper, invoiceTypeId, partnerId, beforeDate, lastInvoices);
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

        private bool InsertBillingTransactions(IEnumerable<GeneratedInvoiceBillingTransaction> billingTransactions, long invoiceId, Vanrise.AccountBalance.Data.SQL.BillingTransactionDataManager billingTransactionDataManager)
        {
            IEnumerable<Vanrise.AccountBalance.Entities.BillingTransaction> mappedTransactions = MapGeneratedInvoiceBillingTransactions(billingTransactions);

            long transactionId;
            bool areAllInsertionsSuccessful = true;

            foreach (Vanrise.AccountBalance.Entities.BillingTransaction mappedTransaction in mappedTransactions)
            {
                bool isInsertionSuccessful = billingTransactionDataManager.Insert(mappedTransaction, invoiceId, out transactionId);
                if (!isInsertionSuccessful)
                    areAllInsertionsSuccessful = false;
            }

            return areAllInsertionsSuccessful;
        }

        private IEnumerable<Vanrise.AccountBalance.Entities.BillingTransaction> MapGeneratedInvoiceBillingTransactions(IEnumerable<GeneratedInvoiceBillingTransaction> billingTransactions)
        {
            return billingTransactions.MapRecords(x =>
            {
                var billingTransaction = new AccountBalance.Entities.BillingTransaction()
                {
                    AccountTypeId = x.AccountTypeId,
                    AccountId = x.AccountId,
                    TransactionTypeId = x.TransactionTypeId,
                    Amount = x.Amount,
                    CurrencyId = x.CurrencyId,
                    TransactionTime = x.TransactionTime,
                    Notes = x.Notes,
                    Reference = x.Reference
                };

                if (x.Settings != null)
                {
                    billingTransaction.Settings = new AccountBalance.Entities.BillingTransactionSettings();

                    if (x.Settings.UsageOverrides != null)
                    {
                        billingTransaction.Settings.UsageOverrides = new List<AccountBalance.Entities.BillingTransactionUsageOverride>();

                        foreach (GeneratedInvoiceBillingTransactionUsageOverride usageOverride in x.Settings.UsageOverrides)
                        {
                            billingTransaction.Settings.UsageOverrides.Add(new AccountBalance.Entities.BillingTransactionUsageOverride()
                            {
                                TransactionTypeId = usageOverride.TransactionTypeId,
                                FromDate = usageOverride.FromDate,
                                ToDate = usageOverride.ToDate
                            });
                        }
                    }
                }

                return billingTransaction;
            });
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
                TimeZoneId = GetReaderValue<int?>(reader, "TimeZoneId"),
                TimeZoneOffset = reader["TimeZoneOffset"] as string,
                SourceId = reader["SourceID"] as string
            };
            return invoice;
        }
        #endregion
    }
}

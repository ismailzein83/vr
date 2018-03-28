using System;
using System.Data;
using System.Collections.Generic;
using Vanrise.Data.SQL;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data.SQL
{
    class InvoiceGenerationDraftDataManager : BaseSQLDataManager, IInvoiceGenerationDraftDataManager
    {
        public InvoiceGenerationDraftDataManager()
            : base(GetConnectionStringName("InvoiceDBConnStringKey", "InvoiceDBConnString"))
        {

        }

        public List<InvoiceGenerationDraft> GetInvoiceGenerationDrafts(Guid invoiceGenerationIdentifier)
        {
            return GetItemsSP("VR_Invoice.sp_InvoiceGenerationDraft_GetAll", InvoiceGenerationDraftMapper, invoiceGenerationIdentifier);
        }

        public bool InsertInvoiceGenerationDraft(InvoiceGenerationDraft invoiceGenerationDraft, out long insertedId)
        {
            object invoiceGenerationDraftId;
            int recordsEffected = ExecuteNonQuerySP("VR_Invoice.sp_InvoiceGenerationDraft_Insert", out invoiceGenerationDraftId, invoiceGenerationDraft.InvoiceGenerationIdentifier,
                invoiceGenerationDraft.InvoiceTypeId, invoiceGenerationDraft.PartnerId, invoiceGenerationDraft.PartnerName, invoiceGenerationDraft.From, invoiceGenerationDraft.To,
                invoiceGenerationDraft.CustomPayload != null ? Vanrise.Common.Serializer.Serialize(invoiceGenerationDraft.CustomPayload) : DBNull.Value);

            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
                insertedId = (long)invoiceGenerationDraftId;
            else
                insertedId = 0;
            return insertedSuccesfully;
        }

        public void DeleteInvoiceGenerationDraft(long invoiceGenerationDraftId)
        {
            ExecuteNonQuerySP("VR_Invoice.sp_InvoiceGenerationDraft_Delete", invoiceGenerationDraftId);
        }
        public void ClearInvoiceGenerationDrafts(Guid invoiceGenerationIdentifier)
        {
            ExecuteNonQuerySP("VR_Invoice.sp_InvoiceGenerationDraft_Clear", invoiceGenerationIdentifier);
        }

        public bool UpdateInvoiceGenerationDraft(InvoiceGenerationDraftToEdit invoiceGenerationDraft)
        {
            int recordsEffected = ExecuteNonQuerySP("VR_Invoice.sp_InvoiceGenerationDraft_Update", invoiceGenerationDraft.InvoiceGenerationDraftId, invoiceGenerationDraft.From,
                invoiceGenerationDraft.To, invoiceGenerationDraft.CustomPayload != null ? Vanrise.Common.Serializer.Serialize(invoiceGenerationDraft.CustomPayload) : DBNull.Value);

            return (recordsEffected > 0);
        }

        public InvoiceGenerationDraftSummary GetInvoiceGenerationDraftsSummary(Guid invoiceGenerationIdentifier)
        {
            return GetItemSPCmd("VR_Invoice.sp_InvoiceGenerationDraft_GetSummary", InvoiceGenerationDraftSummaryMapper, (cmd) =>
             {
                 var invoiceGenerationIdentifierParam = new System.Data.SqlClient.SqlParameter("@InvoiceGenerationIdentifier", SqlDbType.UniqueIdentifier);
                 invoiceGenerationIdentifierParam.Value = invoiceGenerationIdentifier;
                 cmd.Parameters.Add(invoiceGenerationIdentifierParam);
             });
        }

        private InvoiceGenerationDraft InvoiceGenerationDraftMapper(IDataReader reader)
        {
            string customPayload = reader["CustomPayload"] as string;
            InvoiceGenerationDraft invoiceGenerationDraft = new InvoiceGenerationDraft()
            {
                InvoiceGenerationDraftId = (long)reader["ID"],
                InvoiceGenerationIdentifier = (Guid)reader["invoiceGenerationIdentifier"],
                InvoiceTypeId = (Guid)reader["InvoiceTypeId"],
                PartnerId = reader["PartnerId"] as string,
                PartnerName = reader["PartnerName"] as string,
                From = (DateTime)reader["FromDate"],
                To = (DateTime)reader["ToDate"],
                CustomPayload = !string.IsNullOrEmpty(customPayload) ? Vanrise.Common.Serializer.Deserialize(customPayload) : null
            };
            return invoiceGenerationDraft;
        }

        private InvoiceGenerationDraftSummary InvoiceGenerationDraftSummaryMapper(IDataReader reader)
        {
            InvoiceGenerationDraftSummary invoiceGenerationDraftSummary = new InvoiceGenerationDraftSummary()
            {
                TotalCount = (int)reader["TotalCount"],
                MinimumFrom = GetReaderValue<DateTime?>(reader, "MinimumFrom"),
                MaximumTo = GetReaderValue<DateTime?>(reader, "MaximumTo"),
            };
            return invoiceGenerationDraftSummary;
        }

        public InvoiceGenerationDraft GetInvoiceGenerationDraft(long invoiceGenerationDraftId)
        {
            return GetItemSP("VR_Invoice.sp_InvoiceGenerationDraft_Get", InvoiceGenerationDraftMapper, invoiceGenerationDraftId);
        }
    }
}

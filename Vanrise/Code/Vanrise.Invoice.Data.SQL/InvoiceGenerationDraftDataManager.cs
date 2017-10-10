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

        public List<InvoiceGenerationDraft> GetInvoiceGenerationDrafts(int userId, Guid invoiceTypeId)
        {
            return GetItemsSP("VR_Invoice.sp_InvoiceGenerationDraft_GetAll", InvoiceGenerationDraftMapper, userId, invoiceTypeId);
        }

        public bool InsertInvoiceGenerationDraft(int userId, InvoiceGenerationDraft invoiceGenerationDraft, out long insertedId)
        {
            object invoiceGenerationDraftId;
            int recordsEffected = ExecuteNonQuerySP("VR_Invoice.sp_InvoiceGenerationDraft_Insert", out invoiceGenerationDraftId, userId, invoiceGenerationDraft.InvoiceTypeId, 
                invoiceGenerationDraft.PartnerId, invoiceGenerationDraft.PartnerName, invoiceGenerationDraft.From, invoiceGenerationDraft.To,
                invoiceGenerationDraft.CustomPayload != null ? Vanrise.Common.Serializer.Serialize(invoiceGenerationDraft.CustomPayload) : DBNull.Value);

            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
                insertedId = (long)invoiceGenerationDraftId;
            else
                insertedId = 0;
            return insertedSuccesfully;
        }

        public void DeleteInvoiceGenerationDraft(int userId, Guid invoiceTypeId)
        {
            ExecuteNonQuerySP("VR_Invoice.sp_InvoiceGenerationDraft_Delete", userId, invoiceTypeId);
        }

        public bool UpdateInvoiceGenerationDraft(InvoiceGenerationDraft invoiceGenerationDraft)
        {
            throw new NotImplementedException();
        }

        private InvoiceGenerationDraft InvoiceGenerationDraftMapper(IDataReader reader)
        {
            string customPayload = reader["CustomPayload"] as string;
            InvoiceGenerationDraft invoiceGenerationDraft = new InvoiceGenerationDraft()
            {
                InvoiceGenerationDraftId = (long)reader["ID"],
                InvoiceTypeId = (Guid)reader["InvoiceTypeId"],
                PartnerId = reader["PartnerId"] as string,
                PartnerName = reader["PartnerName"] as string,
                From = (DateTime)reader["FromDate"],
                To = (DateTime)reader["ToDate"],
                CustomPayload = !string.IsNullOrEmpty(customPayload) ? Vanrise.Common.Serializer.Deserialize(customPayload) : null
            };
            return invoiceGenerationDraft;
        }
    }
}

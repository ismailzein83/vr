using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data.SQL
{
    public class InvoiceBulkActionsDraftDataManager : BaseSQLDataManager, IInvoiceBulkActionsDraftDataManager
    {
       
        #region ctor
        public InvoiceBulkActionsDraftDataManager()
            : base(GetConnectionStringName("InvoiceDBConnStringKey", "InvoiceDBConnString"))
        {

        }
        #endregion

        public void LoadInvoicesFromInvoiceBulkActionDraft(Guid invoiceBulkActionIdentifier, Action<Entities.Invoice> onInvoiceReady)
        {

            ExecuteReaderSP("[VR_Invoice].[sp_InvoiceBulkActionDraft_LoadInvoices]",
                (reader) =>
                {
                    InvoiceDataManager invoiceDataManager = new InvoiceDataManager();
                    while (reader.Read())
                    {
                        onInvoiceReady(invoiceDataManager.InvoiceMapper(reader));
                    }
                }, invoiceBulkActionIdentifier);
        }


        public InvoiceBulkActionsDraftSummary UpdateInvoiceBulkActionDraft(Guid invoiceBulkActionIdentifier, Guid invoiceTypeId, bool isAllInvoicesSelected, List<long> targetInvoicesIds)
        {
            string targetInvoicesIdsAsString = null;
            if (targetInvoicesIds != null)
                targetInvoicesIdsAsString = string.Join(",", targetInvoicesIds);

            return GetItemSP("VR_Invoice.sp_InvoiceBulkActionDraft_Update", InvoiceBulkActionsDraftSummary, invoiceBulkActionIdentifier, invoiceTypeId, isAllInvoicesSelected, targetInvoicesIdsAsString);
        }


        public void ClearInvoiceBulkActionDrafts(Guid invoiceBulkActionIdentifier)
        {
            ExecuteNonQuerySP("VR_Invoice.sp_InvoiceBulkActionDraft_Clear", invoiceBulkActionIdentifier);
        }

        private InvoiceBulkActionsDraftSummary InvoiceBulkActionsDraftSummary(IDataReader reader)
        {
            return new InvoiceBulkActionsDraftSummary()
            {
                TotalCount = (int)reader["TotalCount"],
                MinimumFrom = GetReaderValue<DateTime?>(reader, "MinimumFrom"),
                MaximumTo = GetReaderValue<DateTime?>(reader, "MaximumTo"),
            };
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data
{
    public interface IInvoiceGenerationDraftDataManager : IDataManager
    {
        List<InvoiceGenerationDraft> GetInvoiceGenerationDrafts(Guid invoiceGenerationIdentifier);
        bool InsertInvoiceGenerationDraft(InvoiceGenerationDraft invoiceGenerationDraft, out long insertedId);
        bool UpdateInvoiceGenerationDraft(InvoiceGenerationDraftToEdit invoiceGenerationDraft);
        void DeleteInvoiceGenerationDraft(long invoiceGenerationDraftId);
        InvoiceGenerationDraft GetInvoiceGenerationDraft(long invoiceGenerationDraftId);
        void ClearInvoiceGenerationDrafts(Guid invoiceGenerationIdentifier);
        InvoiceGenerationDraftSummary GetInvoiceGenerationDraftsSummary(Guid invoiceGenerationIdentifier);
    }
}
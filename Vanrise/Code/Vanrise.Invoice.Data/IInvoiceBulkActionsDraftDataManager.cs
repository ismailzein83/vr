using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data
{
    public interface IInvoiceBulkActionsDraftDataManager:IDataManager
    {
        void LoadInvoicesFromInvoiceBulkActionDraft(Guid invoiceBulkActionIdentifier, Action<Entities.Invoice> onInvoiceReady);
        InvoiceBulkActionsDraftSummary UpdateInvoiceBulkActionDraft(Guid invoiceBulkActionIdentifier, Guid invoiceTypeId, bool isAllInvoicesSelected, List<long> targetInvoicesIds);
        void ClearInvoiceBulkActionDrafts(Guid invoiceBulkActionIdentifier);
    }
}

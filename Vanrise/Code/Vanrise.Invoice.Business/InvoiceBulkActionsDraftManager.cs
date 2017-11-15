using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Invoice.Data;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business
{
    public class InvoiceBulkActionsDraftManager
    {
        public void LoadInvoicesFromInvoiceBulkActionDraft(Guid invoiceBulkActionIdentifier, Action<Entities.Invoice> onInvoiceReady)
        {
            IInvoiceBulkActionsDraftDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceBulkActionsDraftDataManager>();
            dataManager.LoadInvoicesFromInvoiceBulkActionDraft(invoiceBulkActionIdentifier, onInvoiceReady);
        }

        public InvoiceBulkActionsDraftSummary UpdateInvoiceBulkActionDraft(Guid invoiceBulkActionIdentifier, Guid invoiceTypeId, bool isAllInvoicesSelected, List<long> targetInvoicesIds)
        {
            IInvoiceBulkActionsDraftDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceBulkActionsDraftDataManager>();
            return dataManager.UpdateInvoiceBulkActionDraft(invoiceBulkActionIdentifier,invoiceTypeId, isAllInvoicesSelected, targetInvoicesIds);
        }
        public void ClearInvoiceBulkActionDrafts(Guid invoiceBulkActionIdentifier)
        {
            IInvoiceBulkActionsDraftDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceBulkActionsDraftDataManager>();
            dataManager.ClearInvoiceBulkActionDrafts(invoiceBulkActionIdentifier);
        }
    }
}

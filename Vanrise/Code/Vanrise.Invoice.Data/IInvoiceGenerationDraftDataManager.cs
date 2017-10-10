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
        List<InvoiceGenerationDraft> GetInvoiceGenerationDrafts(int userId, Guid invoiceTypeId);
        bool InsertInvoiceGenerationDraft(int userId, InvoiceGenerationDraft invoiceGenerationDraft, out long insertedId);
        bool UpdateInvoiceGenerationDraft(InvoiceGenerationDraft invoiceGenerationDraft);
        void DeleteInvoiceGenerationDraft(int userId, Guid invoiceTypeId);
    }
}

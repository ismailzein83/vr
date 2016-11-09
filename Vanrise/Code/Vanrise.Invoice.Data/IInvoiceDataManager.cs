using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data
{
    public interface IInvoiceDataManager:IDataManager
    {
        IEnumerable<Entities.Invoice> GetFilteredInvoices(DataRetrievalInput<InvoiceQuery> input);
        int GetInvoiceCount(Guid InvoiceTypeId, string partnerId, DateTime? fromDate, DateTime? toDate);
        bool SaveInvoices(GenerateInvoiceInput createInvoiceInput, GeneratedInvoice invoice,Entities.Invoice invoiceEntity, out long insertedInvoiceId);
        Entities.Invoice GetInvoice(long invoiceId);
        bool SetInvoicePaid(long invoiceId, DateTime? paidDate);
    }
}

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
        IEnumerable<Entities.Invoice> GetGetFilteredInvoices(DataRetrievalInput<InvoiceQuery> input);
        void SaveInvoices(GenerateInvoiceInput createInvoiceInput, GeneratedInvoice invoice);
        Entities.Invoice GetInvoice(long invoiceId);
    }
}

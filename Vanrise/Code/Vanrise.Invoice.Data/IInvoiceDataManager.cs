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
        List<Entities.InvoiceDetail> GetInvoices(DataRetrievalInput<InvoiceQuery> input);
        void SaveInvoices(GenerateInvoiceInput createInvoiceInput, GeneratedInvoice invoice);
        bool AreInvoicesUpdated(ref object updateHandle);
    }
}

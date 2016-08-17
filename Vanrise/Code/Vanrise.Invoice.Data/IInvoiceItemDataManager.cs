using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data
{
    public interface IInvoiceItemDataManager:IDataManager
    {
        IEnumerable<Entities.InvoiceItem> GetFilteredInvoiceItems(DataRetrievalInput<InvoiceItemQuery> input);
        IEnumerable<InvoiceItem> GetInvoiceItemsByItemSetNames(long invoiceId, List<string> itemSetNames);
    }
}

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
        string StorageConnectionStringKey { set; }
        IEnumerable<Entities.InvoiceItem> GetFilteredInvoiceItems(long invoiceId, string itemSetNam, CompareOperator compareOperator);
        IEnumerable<InvoiceItem> GetInvoiceItemsByItemSetNames(List<long> invoiceIds, IEnumerable<string> itemSetNames, CompareOperator compareOperator);
    }
}

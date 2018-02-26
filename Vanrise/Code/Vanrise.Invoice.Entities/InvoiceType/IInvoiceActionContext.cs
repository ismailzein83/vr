using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public interface IInvoiceActionContext
    {
        Invoice GetInvoice { get; }

        IEnumerable<InvoiceItem> GetInvoiceItems(List<string> itemSetNames, CompareOperator CompareOperator);

        bool DoesUserHaveAccess(Guid invoiceActionId);
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Invoice.Entities
{

    public class InvoiceActionPayloadSettings : VRTempPayloadSettings
    {
        public IInvoiceActionContext Context { get; set; }
    }

    public interface IInvoiceActionContext
    {
       Invoice GetInvoice { get; }

       IEnumerable<InvoiceItem> GetInvoiceItems(List<string> itemSetNames, CompareOperator CompareOperator);

       bool DoesUserHaveAccess(Guid invoiceActionId);
    }

}

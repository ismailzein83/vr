using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public interface IInvoiceSettingManager:IBusinessManager
    {
        InvoiceSetting GetDefaultInvoiceSetting(Guid invoiceTypeId);
    }
}

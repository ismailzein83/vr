using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public interface IInvoiceSettingManager:IBusinessManager
    {
        InvoiceSetting GetInvoiceSetting(Guid invoiceSettingId);
        InvoiceSetting GetDefaultInvoiceSetting(Guid invoiceTypeId);
        T GetInvoiceSettingDetailByType<T>(Guid invoiceSettingId) where T : InvoiceSettingPart;
    }
}

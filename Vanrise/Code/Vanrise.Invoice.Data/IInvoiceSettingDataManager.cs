using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data
{
    public interface IInvoiceSettingDataManager:IDataManager
    {
        List<InvoiceSetting> GetInvoiceSettings();
        bool AreInvoiceSettingsUpdated(ref object updateHandle);
        bool InsertInvoiceSetting(InvoiceSetting invoiceSetting);
        bool UpdateInvoiceSetting(InvoiceSetting invoiceSetting);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Business.Extensions;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class TaxesDataSourceSettings : InvoiceDataSourceSettings
    {
        public override Guid ConfigId { get { return new Guid("B0F8D3C8-33C3-4FA6-AB7C-016C14575F47"); } }
        public override IEnumerable<dynamic> GetDataSourceItems(IInvoiceDataSourceSettingsContext context)
        {
            var invoice = context.InvoiceActionContext.GetInvoice();
            var invoiceType = new InvoiceTypeManager().GetInvoiceType(invoice.InvoiceTypeId);
            InvoiceTypeExtendedSettingsInfoContext InvoiceTypeExtendedSettingsInfoContext = new InvoiceTypeExtendedSettingsInfoContext
            {
                InfoType = "Taxes",
                Invoice =invoice
            };
           return invoiceType.Settings.ExtendedSettings.GetInfo(InvoiceTypeExtendedSettingsInfoContext);
        }
    }
}

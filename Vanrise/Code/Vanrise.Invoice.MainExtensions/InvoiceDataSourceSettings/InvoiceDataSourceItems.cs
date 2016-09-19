using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class InvoiceDataSourceItems : InvoiceDataSourceSettings
    {
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("65D64951-AD27-475C-81D2-FAC7816E6E4B"); } }
        public override IEnumerable<dynamic> GetDataSourceItems(IInvoiceDataSourceSettingsContext context)
        {
            List<dynamic> invoices = new List<dynamic>();
            if(context.InvoiceActionContext != null)
            {
                var invoice = context.InvoiceActionContext.GetInvoice;
                invoices.Add(invoice.Details);
            }
            return invoices;
        }
    }
}

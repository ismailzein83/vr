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

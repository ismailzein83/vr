using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business
{
    public class TOneCustomerInvoiceType : InvoiceTypeSettings
    {
        public override void CreateInvoice(IInvoiceCreationContext context)
        {
            throw new NotImplementedException();
        }
    }
}

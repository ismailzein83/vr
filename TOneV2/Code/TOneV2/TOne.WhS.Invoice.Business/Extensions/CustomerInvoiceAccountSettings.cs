using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Invoice.Entities;
using Vanrise.Invoice.Business;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public class CustomerInvoiceAccountSettings : InvoiceAccountExtendedSettings
    {
        InvoiceTypeManager _invoiceTypeManager = new InvoiceTypeManager();
        public override bool IsCustomerAccount(IInvoiceAccountIsCustomerAccountContext context)
        {
            return true;
        }

        public override bool IsSupplierAccount(IInvoiceAccountIsSupplierAccountContext context)
        {
            return false;
        }
    }
}

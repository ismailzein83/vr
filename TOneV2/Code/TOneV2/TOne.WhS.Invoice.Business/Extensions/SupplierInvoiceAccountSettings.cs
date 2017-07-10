using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Invoice.Entities;
using Vanrise.Invoice.Business;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public class SupplierInvoiceAccountSettings : InvoiceAccountExtendedSettings
    {
        InvoiceTypeManager _invoiceTypeManager = new InvoiceTypeManager();
        public override bool IsCustomerAccount(IInvoiceAccountIsCustomerAccountContext context)
        {
            return false;
        }

        public override bool IsSupplierAccount(IInvoiceAccountIsSupplierAccountContext context)
        {
            return true;
        }
    }
}

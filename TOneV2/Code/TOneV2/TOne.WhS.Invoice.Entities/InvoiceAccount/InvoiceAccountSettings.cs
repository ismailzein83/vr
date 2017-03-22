using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class InvoiceAccountSettings
    {
        public Guid InvoiceTypeId { get; set; }

        public InvoiceAccountExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class InvoiceAccountExtendedSettings
    {
        public abstract bool IsCustomerAccount(IInvoiceAccountIsCustomerAccountContext context);

        public abstract bool IsSupplierAccount(IInvoiceAccountIsSupplierAccountContext context);
    }

    public interface IInvoiceAccountIsCustomerAccountContext
    {
        Guid InvoiceTypeId { get; }
    }

    public interface IInvoiceAccountIsSupplierAccountContext
    {
        Guid InvoiceTypeId { get; }
    }

    public class InvoiceAccountIsCustomerAccountContext : IInvoiceAccountIsCustomerAccountContext
    {
        public Guid InvoiceTypeId { get; set; }
    }

    public class InvoiceAccountIsSupplierAccountContext : IInvoiceAccountIsSupplierAccountContext
    {
        public Guid InvoiceTypeId { get; set; }
    }

}

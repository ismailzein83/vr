using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business
{
    public abstract class InvoiceSettings : InvoiceTypeExtendedSettings
    {
        public abstract bool IsApplicableToCustomer { get; }

        public abstract bool IsApplicableToSupplier { get; }

        public virtual string RuntimeEditor { get; set; }
    }
}

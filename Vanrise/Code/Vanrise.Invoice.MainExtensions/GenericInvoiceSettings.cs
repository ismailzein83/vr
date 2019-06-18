using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public abstract class GenericInvoiceSettings : InvoiceTypeExtendedSettings
    {
        public Guid? InvoiceTransactionTypeId { get; set; }
        public List<Guid> UsageToOverrideTransactionTypeIds { get; set; }
        public GenericFinancialAccountConfiguration Configuration { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business.Extensions
{
    public class InvoiceTypeExtendedSettingsInfoContext : IInvoiceTypeExtendedSettingsInfoContext
    {
        public string InfoType { get; set; }
        public Entities.Invoice Invoice { get; set; }
    }
}

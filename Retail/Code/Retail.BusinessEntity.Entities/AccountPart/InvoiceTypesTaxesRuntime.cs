using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class InvoiceTypesTaxesRuntime
    {
        public string InvoiceTypeTitle { get; set; }
        public VRTaxesDefinition TaxesDefinitions { get; set; }
    }
}

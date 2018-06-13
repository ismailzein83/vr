using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class TaxInvoiceTypeSetting
    {
        public Guid InvoiceTypeId { get; set; }
        public Vanrise.Entities.VRTaxSetting VRTaxSetting { get; set; }
    }
}

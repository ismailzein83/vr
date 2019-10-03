using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public class RetailBillingChargeTypeInfo
    {
        public Guid RetailBillingChargeTypeId { get; set; }
        public string Name { get; set; }
        public string RuntimeEditor { get; set; }
    }
}

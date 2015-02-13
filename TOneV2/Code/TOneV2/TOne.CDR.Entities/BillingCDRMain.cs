using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.CDR.Entities
{
    public class BillingCDRMain : BillingCDRBase
    {
        public BillingCDRCost cost { get; set; }

        public BillingCDRSale sale { get; set; }
    }

}

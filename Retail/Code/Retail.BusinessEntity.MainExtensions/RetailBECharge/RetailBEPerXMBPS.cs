using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.RetailBECharge
{
    public class RetailBEPerXMBPS : RetailBEChargeSettings
    {
        public override Guid ConfigId { get { return new Guid("1151BAD9-E4E1-450A-B7BA-589C1940D7BE"); } }
        public decimal ChargeValue { get; set; }
        public decimal MBPS { get; set; }
    }
}

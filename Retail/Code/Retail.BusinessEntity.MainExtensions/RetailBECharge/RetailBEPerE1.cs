using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.RetailBECharge
{
    public class RetailBEPerE1 : RetailBEChargeSettings
    {
        public override Guid ConfigId { get { return new Guid("CEEAE225-FCC4-49FD-BCEF-D24C6BBB52AE"); } }
        public decimal ChargeValue { get; set; }

        public override string GetDescription()
        {
            return string.Format("{0:#,0.##} * Nb of E1", ChargeValue);
        }
    }
}

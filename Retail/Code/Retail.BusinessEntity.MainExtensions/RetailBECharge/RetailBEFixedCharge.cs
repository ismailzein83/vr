using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.MainExtensions.RetailBECharge
{
    public class RetailBEFixedCharge : RetailBEChargeSettings
    {
        public override Guid ConfigId { get { return new Guid("0DEC0B45-5A1E-4A91-834F-1AADD591E1E9"); } }
        public decimal ChargeValue { get; set; }

        public override string GetDescription()
        {
            return string.Format("Fixed charge value is: {0}", ChargeValue);
        }
    }
}

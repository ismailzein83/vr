using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public class RetailBillingChargeTypeSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId => new Guid("91DE3029-2EBE-4B7F-8503-E50E614F5832");

        public RetailBillingChargeTypeExtendedSettings ExtendedSettings { get; set; }
    }
}
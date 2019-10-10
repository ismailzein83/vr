using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public class DiscountRuleTargetTypeSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId => throw new NotImplementedException();

        public DiscountRuleTargetTypeExtendedSettings ExtendedSettings { get; set; }
    }
}
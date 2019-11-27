using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public class DiscountRuleTargetTypeSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId => new Guid("91441947-14A0-4AB6-AC7E-A023BF9C809B");

        public Guid TargetRecordTypeId { get; set; }

    }
}
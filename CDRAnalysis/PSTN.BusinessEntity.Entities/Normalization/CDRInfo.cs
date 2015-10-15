using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public class CDRToNormalizeInfo : Vanrise.Rules.BaseRuleTargetIdentifier
    {
        public int? SwitchId { get; set; }

        public int? TrunkId { get; set; }

        public string PhoneNumber { get; set; }

        public NormalizationPhoneNumberType PhoneNumberType { get; set; }
    }
}

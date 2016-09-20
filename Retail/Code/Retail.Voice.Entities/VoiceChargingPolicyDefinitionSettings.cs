using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Voice.Entities
{
    public class VoiceChargingPolicyDefinitionSettings : ChargingPolicyDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("6ba989f8-71d6-42dc-80f0-5128ebb8ffd2"); } }

        public override string ChargingPolicyEditor
        {
            get
            {
                return "retail-voice-chargingpolicy-settings";
            }
            set
            {
                
            }
        }
    }
}

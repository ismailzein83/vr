﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Voice.Entities
{
    public class VoiceChargingPolicyDefinitionSettings : ChargingPolicyDefinitionSettings
    {
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

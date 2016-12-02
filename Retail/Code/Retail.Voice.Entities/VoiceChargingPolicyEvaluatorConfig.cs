using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.Voice.Entities
{
    public class VoiceChargingPolicyEvaluatorConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Retail_Voice_VoiceChargingPolicyEvaluatorConfig";

        public string Editor { get; set; }
    }
}

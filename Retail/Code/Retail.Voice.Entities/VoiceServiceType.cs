using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Voice.Entities
{
    public class VoiceServiceType : ServiceTypeExtendedSettings, IServiceVoiceChargingPolicyEvaluator
    {
        public override Guid ConfigId
        {
            get { return new Guid("2FF81206-1E07-4E66-9E35-7F53BF049AB3"); }
        }

        public VoiceChargingPolicyEvaluator VoiceChargingPolicyEvaluator { get; set; }

        public VoiceChargingPolicyEvaluator GetChargingPolicyEvaluator()
        {
            return this.VoiceChargingPolicyEvaluator;
        }
    }
}

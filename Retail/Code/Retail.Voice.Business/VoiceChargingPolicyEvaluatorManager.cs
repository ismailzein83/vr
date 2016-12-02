using Retail.Voice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace Retail.Voice.Business
{
    public class VoiceChargingPolicyEvaluatorManager
    {
        #region Public Methods

        public IEnumerable<VoiceChargingPolicyEvaluatorConfig> GetVoiceChargingPolicyEvaluatorTemplateConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<VoiceChargingPolicyEvaluatorConfig>(VoiceChargingPolicyEvaluatorConfig.EXTENSION_TYPE); 
        }

        #endregion
    }
}

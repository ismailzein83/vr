using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class ReceivedRequestLogManager
    {
        #region Public Methods
        public IEnumerable<VRReceivedRequestLogModuleFilterConfig> GetReceivedRequestLogFilterModuleConfigs()
        {
            Common.Business.ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<VRReceivedRequestLogModuleFilterConfig>(VRReceivedRequestLogModuleFilterConfig.EXTENSION_TYPE);
        }
        #endregion
    }
}

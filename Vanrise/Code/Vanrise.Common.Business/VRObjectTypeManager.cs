using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRObjectTypeManager
    {
        #region Public Methods

        public IEnumerable<VRObjectTypeConfig> GetObjectTypeExtensionConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<VRObjectTypeConfig>(VRObjectTypeConfig.EXTENSION_TYPE);
        }

        #endregion
    }
}

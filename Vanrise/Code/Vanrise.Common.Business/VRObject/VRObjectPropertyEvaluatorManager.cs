using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRObjectPropertyEvaluatorManager
    {
        #region Public Methods

        public IEnumerable<VRObjectPropertyEvaluatorConfig> GetObjectPropertyExtensionConfigs(string configType)
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<VRObjectPropertyEvaluatorConfig>(configType);
        }

        #endregion
    }
}

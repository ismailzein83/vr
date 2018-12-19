using System.Collections.Generic;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class CompositeRecordConditionManager
    {
        public IEnumerable<CompositeRecordConditionConfig> GetCompositeRecordConditionConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<CompositeRecordConditionConfig>(CompositeRecordConditionConfig.EXTENSION_TYPE);
        }
    }
}

using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
	public class CompositeRecordConditionDefinitionManager
    {
        public IEnumerable<CompositeRecordConditionDefinitionSettingConfig> GetCompositeRecordConditionDefinitionSettingConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<CompositeRecordConditionDefinitionSettingConfig>(CompositeRecordConditionDefinitionSettingConfig.EXTENSION_TYPE);
        }
    }
}

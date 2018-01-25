using System;
using System.Collections.Generic;
using Vanrise.Common.Business;
using Vanrise.Reprocess.Entities;

namespace Vanrise.Reprocess.Business
{
    public class ReprocessFilterDefinitionManager
    {
        public IEnumerable<ReprocessFilterDefinitionConfig> GetReprocessFilterDefinitionConfigs()
        {
            return new ExtensionConfigurationManager().GetExtensionConfigurations<ReprocessFilterDefinitionConfig>(ReprocessFilterDefinitionConfig.EXTENSION_TYPE);
        }
    }
}

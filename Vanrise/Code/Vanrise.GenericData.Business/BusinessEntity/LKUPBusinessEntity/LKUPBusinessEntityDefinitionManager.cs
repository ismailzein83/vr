using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.GenericData.Business
{
    public class LKUPBusinessEntityDefinitionManager
    {
        public BusinessEntityDefinition GetLookUPBEDefinition(Guid businessEntityDefinitionId)
        {
            BusinessEntityDefinitionManager manager = new BusinessEntityDefinitionManager();
            var lookUpBEDefinition = manager.GetBusinessEntityDefinition(businessEntityDefinitionId);
            lookUpBEDefinition.ThrowIfNull("lookUpBEDefinition", businessEntityDefinitionId);
            return lookUpBEDefinition;
        }

        public string GetLookUpBEDefinitionName(Guid businessEntityDefinitionId)
        {
            return GetLookUPBEDefinition(businessEntityDefinitionId).Name;
        }
        public string GetLookUpBEDefinitionTitle(Guid businessEntityDefinitionId)
        {
            return GetLookUPBEDefinition(businessEntityDefinitionId).Title;
        }

        public LKUPBusinessEntityDefinitionSettings GetLookUpBEDefinitionSettings(Guid businessEntityDefinitionId)
        {
            var lookUpBEDefinition = GetLookUPBEDefinition(businessEntityDefinitionId);
            lookUpBEDefinition.Settings.ThrowIfNull("lookUpBEDefinition.Settings");
            return lookUpBEDefinition.Settings.CastWithValidate<LKUPBusinessEntityDefinitionSettings>("lookUpBEDefinition.Settings");
        }

        public LKUPBESelectorRuntimeInfo GetLookUpBESelectorRuntimeInfo(Guid businessEntityDefinitionId)
        {
            var lookUpBEDefinitionSettings = GetLookUpBEDefinitionSettings(businessEntityDefinitionId);
            lookUpBEDefinitionSettings.ThrowIfNull("lookUpBEDefinitionSettings");
            return new LKUPBESelectorRuntimeInfo
            {
                SelectorPluralTitle = lookUpBEDefinitionSettings.SelectorPluralTitle,
                SelectorSingularTitle = lookUpBEDefinitionSettings.SelectorSingularTitle
            };
        }

        public IEnumerable<LKUPBEExtendedSettingsConfig> GetLookUpBEExtendedSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<LKUPBEExtendedSettingsConfig>(LKUPBEExtendedSettingsConfig.EXTENSION_TYPE);
        }

      
    }
}

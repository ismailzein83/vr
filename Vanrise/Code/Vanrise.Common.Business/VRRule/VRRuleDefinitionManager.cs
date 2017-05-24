using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRRuleDefinitionManager
    {
        #region Public Methods

        public IEnumerable<VRRuleDefinitionExtendedSettingsConfig> GetVRRuleDefinitionExtendedSettingsConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<VRRuleDefinitionExtendedSettingsConfig>(VRRuleDefinitionExtendedSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<VRRuleDefinitionInfo> GetVRRuleDefinitionsInfo(VRRuleDefinitionInfoFilter filter)
        {
            Dictionary<Guid, VRRuleDefinition> cachedVRRuleDefinitions = GetCachedVRRuleDefinitions();

            Func<VRRuleDefinition, bool> filterExpression = (vrRuleDefinition) =>
            {
                if (filter == null)
                    return true;

                return true;
            };

            return cachedVRRuleDefinitions.MapRecords(VRRuleDefinitionInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        #endregion

        #region Private Methods

        private Dictionary<Guid, VRRuleDefinition> GetCachedVRRuleDefinitions()
        {
            return new VRComponentTypeManager().GetCachedOrCreate("GetCachedVRRuleDefinitions", () =>
            {
                VRComponentTypeManager vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
                return vrComponentTypeManager.GetCachedComponentTypes<VRRuleDefinitionSettings, VRRuleDefinition>();
            });
        }

        private VRRuleDefinitionInfo VRRuleDefinitionInfoMapper(VRRuleDefinition vrRuleDefinition)
        {
            return new VRRuleDefinitionInfo
            {
                VRRuleDefinitionId = vrRuleDefinition.VRComponentTypeId,
                Name = vrRuleDefinition.Name
            };
        }

        #endregion
    }
}

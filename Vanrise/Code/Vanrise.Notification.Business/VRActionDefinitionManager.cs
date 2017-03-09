using System;
using System.Collections.Generic;
using Vanrise.Common.Business;
using Vanrise.Notification.Entities;
using Vanrise.Common;

namespace Vanrise.Notification.Business
{
    public class VRActionDefinitionManager
    {
        public IEnumerable<VRActionDefinitionConfig> GetVRActionDefinitionConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<VRActionDefinitionConfig>(VRActionDefinitionConfig.EXTENSION_TYPE);
        }

        public IEnumerable<VRActionDefinitionInfo> GetVRActionDefinitionsInfo(VRActionDefinitionFilter filter)
        {
            VRActionDefinitionIsApplicableContext context = null;
            if (filter != null && filter.VRActionTargetType != null)
                context = new VRActionDefinitionIsApplicableContext() { Target = filter.VRActionTargetType };

            Func<VRActionDefinition, bool> predicate = (vrActionDefinition) =>
            {
                if (filter != null)
                {
                    if (context != null && !vrActionDefinition.Settings.ExtendedSettings.IsApplicable(context))
                        return false;
                }
                return true;
            };

            return this.GetCachedVRActionDefinitions().MapRecords(VRActionDefinitionInfoMapper, predicate);
        }

        public VRActionDefinition GetVRActionDefinition(Guid vrActionDefinitionId)
        {
            return this.GetCachedVRActionDefinitions().GetRecord(vrActionDefinitionId);
        }

        private VRActionDefinitionInfo VRActionDefinitionInfoMapper(VRActionDefinition vrActionDefinition)
        {
            VRActionDefinitionInfo vrActionDefinitionInfo = new VRActionDefinitionInfo()
            {
                Name = vrActionDefinition.Name,
                VRActionDefinitionId = vrActionDefinition.VRComponentTypeId,
                RuntimeEditor = vrActionDefinition.Settings.ExtendedSettings.RuntimeEditor
            };

            return vrActionDefinitionInfo;
        }

        private Dictionary<Guid, VRActionDefinition> GetCachedVRActionDefinitions()
        {
            VRComponentTypeManager vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
            return vrComponentTypeManager.GetCachedComponentTypes<VRActionDefinitionSettings, VRActionDefinition>();
        }
    }
}
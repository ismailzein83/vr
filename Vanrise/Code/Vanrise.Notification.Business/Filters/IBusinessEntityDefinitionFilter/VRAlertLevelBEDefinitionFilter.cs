using Vanrise.GenericData.Entities;

namespace Vanrise.Notification.Business
{
    public class VRAlertLevelBEDefinitionFilter : IBusinessEntityDefinitionFilter
    {
        public bool IsMatched(IBusinessEntityDefinitionFilterContext context)
        {
            if (context.entityDefinition == null || context.entityDefinition.Settings == null)
                return false;

            if (context.entityDefinition.Settings.ConfigId != VRAlertLevelBEDefinitionSettings.s_configId)
                return false;

            return true;
        }
    }
}

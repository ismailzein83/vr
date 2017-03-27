using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.Business
{
    public class VRAlertLevelBEDefinitionFilter :IBusinessEntityDefinitionFilter
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

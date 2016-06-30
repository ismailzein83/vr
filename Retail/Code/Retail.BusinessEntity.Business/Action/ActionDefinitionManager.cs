using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class ActionDefinitionManager : IActionDefinitionManager
    {
        public ActionDefinition GetActionDefinition(Guid actionDefinitionId)
        {
            throw new NotImplementedException();
        }


        public ActionBPDefinitionSettings GetActionBPDefinitionSettings(Guid actionDefinitionId)
        {
            var actionDefinitionSettings = GetActionDefinitionSettings(actionDefinitionId);
            if (actionDefinitionSettings.BPDefinitionSettings == null)
                throw new NullReferenceException(String.Format("actionDefinition.Settings.BPDefinitionSettings. Id '{0}'", actionDefinitionId));
            return actionDefinitionSettings.BPDefinitionSettings;
        }

        private ActionDefinitionSettings GetActionDefinitionSettings(Guid actionDefinitionId)
        {
            var actionDefinition = GetActionDefinition(actionDefinitionId);
            if (actionDefinition == null)
                throw new NullReferenceException(String.Format("actionDefinition. Id '{0}'", actionDefinitionId));
            if (actionDefinition.Settings == null)
                throw new NullReferenceException(String.Format("actionDefinition.Settings. Id '{0}'", actionDefinitionId));
            return actionDefinition.Settings;
        }
    }
}

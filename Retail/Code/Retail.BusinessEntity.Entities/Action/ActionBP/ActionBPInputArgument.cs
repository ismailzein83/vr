using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class ActionBPInputArgument : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public Guid ActionDefinitionId { get; set; }

        public long ActionEntityId { get; set; }

        public ActionBPSettings ActionBPSettings { get; set; }

        public override string EntityId
        {
            get
            {
                var actionBPDefinition = BEManagerFactory.GetManager<IActionDefinitionManager>().GetActionDefinition(this.ActionDefinitionId);
                if (actionBPDefinition == null)
                    throw new NullReferenceException(String.Format("actionBPDefinition ActionDefinitionId '{0}'", this.ActionDefinitionId));
                if (actionBPDefinition.Settings == null)
                    throw new NullReferenceException(String.Format("actionBPDefinition.Settings ActionDefinitionId '{0}'", this.ActionDefinitionId));
                return String.Format("Retail_BE_{0}_{1}", actionBPDefinition.EntityType, this.ActionEntityId);
            }
        }


        public override string ProcessName
        {
            get
            {
                var actionBPDefinitionSettings = BEManagerFactory.GetManager<IActionDefinitionManager>().GetActionBPDefinitionSettings(this.ActionDefinitionId);
                if (actionBPDefinitionSettings == null)
                    throw new NullReferenceException(String.Format("actionBPDefinitionSettings. ActionDefinitionId '{0}'", this.ActionDefinitionId));
                return String.Format("Retail_BE_ActionBPInputArgument_{0}", actionBPDefinitionSettings.ConfigId);
            }
        }

        public override string GetTitle()
        {
            var actionBPDefinition = BEManagerFactory.GetManager<IActionDefinitionManager>().GetActionDefinition(this.ActionDefinitionId);
            return String.Format("{0} {1}", actionBPDefinition.Name, Vanrise.Common.Utilities.GetEnumDescription<EntityType>(actionBPDefinition.EntityType));
        }
    }
}

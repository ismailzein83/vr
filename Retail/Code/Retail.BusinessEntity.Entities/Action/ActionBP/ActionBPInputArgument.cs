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

        public long EntityId { get; set; }

        public ActionProvisioner ActionProvisioner { get; set; }

        public ActionBPSettings ActionBPSettings { get; set; }

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
            return String.Format("Action Business Process");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Dealers.Entities
{
    public class DealerActionBPInputArgument : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public Guid ActionDefinitionId { get; set; }

        public long DealerId { get; set; }

        public DealerActionBPSettings ActionBPSettings { get; set; }

        public override string EntityId
        {
            get
            {
                return String.Format("Retail_Dealers_{0}", this.DealerId);
            }
        }


        public override string ProcessName
        {
            get
            {
                //var actionBPDefinitionSettings = BEManagerFactory.GetManager<IActionDefinitionManager>().GetActionBPDefinitionSettings(this.ActionDefinitionId);
                //if (actionBPDefinitionSettings == null)
                //    throw new NullReferenceException(String.Format("actionBPDefinitionSettings. ActionDefinitionId '{0}'", this.ActionDefinitionId));
                //return String.Format("Retail_BE_ActionBPInputArgument_{0}", actionBPDefinitionSettings.ConfigId);
                throw new NotImplementedException();
            }
        }

        public override string GetTitle()
        {
            //var actionBPDefinition = BEManagerFactory.GetManager<IActionDefinitionManager>().GetActionDefinition(this.ActionDefinitionId);
            //return String.Format("{0} {1}", actionBPDefinition.Name, Vanrise.Common.Utilities.GetEnumDescription<EntityType>(actionBPDefinition.EntityType));
            throw new NotImplementedException();
        }
    }
}

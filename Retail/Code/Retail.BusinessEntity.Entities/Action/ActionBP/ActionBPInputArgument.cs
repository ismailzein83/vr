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
        public Guid AccountBEDefinitionId { get; set; }
        public long AccountId { get; set; }
     
        public Guid ActionDefinitionId { get; set; }
        public ActionBPSettings ActionBPSettings { get; set; }

        public override string EntityId
        {
            get
            {
                return String.Format("Retail_BE_{0}_{1}", this.AccountBEDefinitionId, this.AccountId);
            }
        }

        public override string ProcessName
        {
            get
            {
                var accountActionDefinition = BEManagerFactory.GetManager<IAccountBEDefinitionManager>().GetAccountActionDefinition(this.AccountBEDefinitionId,this.ActionDefinitionId);
                if (accountActionDefinition == null)
                    throw new NullReferenceException(String.Format("accountActionDefinition AccountActionDefinitionId '{0}'", this.ActionDefinitionId));
                if (accountActionDefinition.ActionDefinitionSettings == null)
                    throw new NullReferenceException(String.Format("accountActionDefinition.ActionDefinitionSettings AccountActionDefinitionId '{0}'", this.ActionDefinitionId));
                return String.Format("Retail_BE_ActionBPInputArgument_{0}", accountActionDefinition.ActionDefinitionSettings.ConfigId);
            }
        }

        public override string GetTitle()
        {
            var accountActionDefinition = BEManagerFactory.GetManager<IAccountBEDefinitionManager>().GetAccountActionDefinition(this.AccountBEDefinitionId, this.ActionDefinitionId);
            if (accountActionDefinition == null)
                throw new NullReferenceException(String.Format("accountActionDefinition AccountActionDefinitionId '{0}'", this.ActionDefinitionId));
            if (accountActionDefinition.ActionDefinitionSettings == null)
                throw new NullReferenceException(String.Format("accountActionDefinition.ActionDefinitionSettings AccountActionDefinitionId '{0}'", this.ActionDefinitionId));
            return String.Format("{0}", accountActionDefinition.Name);
        }
    }
}

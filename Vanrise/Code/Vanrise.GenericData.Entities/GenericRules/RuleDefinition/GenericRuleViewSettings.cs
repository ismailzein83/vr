using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.GenericData.Entities
{
    public class GenericRuleViewSettings : Vanrise.Security.Entities.ViewSettings
    {
        public Guid RuleDefinitionId { get; set; }

        public override string GetURL(Security.Entities.View view)
        {
            return String.Format("#/viewwithparams/VR_GenericData/Views/GenericRule/GenericRuleManagement/{{\"ruleDefinitionId\":\"{0}\"}}", this.RuleDefinitionId);
        }

        public override bool DoesUserHaveAccess(IViewUserAccessContext context)
        {
           return BusinessManagerFactory.GetManager<IGenericRuleDefinitionManager>().DoesUserHaveViewAccess(context.UserId ,this.RuleDefinitionId);

        }
    }
}

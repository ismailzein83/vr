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
        public List<Guid> RuleDefinitionIds { get; set; }

        public override string GetURL(Security.Entities.View view)
        {
            return String.Format("#/viewwithparams/VR_GenericData/Views/GenericRule/GenericRuleManagement/{{\"viewId\":\"{0}\"}}", view.ViewId);
        }

        public override bool DoesUserHaveAccess(IViewUserAccessContext context)
        {
            if (this.RuleDefinitionIds != null)
                return BusinessManagerFactory.GetManager<IGenericRuleDefinitionManager>().DoesUserHaveViewAccess(context.UserId, this.RuleDefinitionIds);
            else
                return false;

        }
    }
}

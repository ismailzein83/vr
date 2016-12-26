using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.GenericData.Entities
{
    public class GenericBEDefinitionSettingsViewSettings : Vanrise.Security.Entities.ViewSettings
    {
        public Guid BusinessEntityDefinitionId { get; set; }
        public override string GetURL(Security.Entities.View view)
        {
            return String.Format("#/viewwithparams/VR_GenericData/Views/GenericBusinessEntity/Runtime/GenericBusinessEntityManagement/{{\"businessEntityDefinitionId\":\"{0}\"}}", this.BusinessEntityDefinitionId);
        }
        public override bool DoesUserHaveAccess(Security.Entities.IViewUserAccessContext context)
        {

            return BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>().DoesUserHaveViewAccess(context.UserId, this.BusinessEntityDefinitionId);
        }
    }
}

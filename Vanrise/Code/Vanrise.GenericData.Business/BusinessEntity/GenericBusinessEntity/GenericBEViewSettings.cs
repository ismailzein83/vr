using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Business
{
    public class GenericBEViewSettings : Vanrise.Security.Entities.ViewSettings
    {
        public List<GenericBEViewSettingItem> Settings { get; set; }

        public override string GetURL(Security.Entities.View view)
        {
            return String.Format("#/viewwithparams/VR_GenericData/Views/GenericBusinessEntity/Runtime/GenericBusinessEntityManagement/{{\"viewId\":\"{0}\"}}", view.ViewId);
        }

        public override bool DoesUserHaveAccess(Security.Entities.IViewUserAccessContext context)
        {

            //return BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>().DoesUserHaveViewAccess(context.UserId, this.BusinessEntityDefinitionId);
            return true;
        }
    }
    public class GenericBEViewSettingItem
    {
        public Guid BusinessEntityDefinitionId { get; set; }
    }
}

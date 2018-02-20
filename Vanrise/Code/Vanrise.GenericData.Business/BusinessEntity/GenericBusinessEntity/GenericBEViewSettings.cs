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

            if (this.Settings.Select(x => x.BusinessEntityDefinitionId).ToList() != null)
                return new GenericBusinessEntityManager().DoesUserHaveViewAccess(context.UserId, this.Settings.Select(x => x.BusinessEntityDefinitionId).ToList());
            return false;
        }
    }
    public class GenericBEViewSettingItem
    {
        public Guid BusinessEntityDefinitionId { get; set; }
    }
}

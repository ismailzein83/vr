using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

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
        public override string GetIconPath()
        {
            if (Settings == null || Settings.Count() == 0)
                return null;

            BusinessEntityDefinitionManager businessEntityDefinitionManager = new BusinessEntityDefinitionManager();

            var firstView = Settings.First();
            var businessEntityDefinition = businessEntityDefinitionManager.GetBusinessEntityDefinition(firstView.BusinessEntityDefinitionId);
            businessEntityDefinition.ThrowIfNull("BusinessEntityDefinition", firstView.BusinessEntityDefinitionId);
            businessEntityDefinition.Settings.ThrowIfNull("BusinessEntityDefinitionSettings", firstView.BusinessEntityDefinitionId);

            return businessEntityDefinition.Settings.IconPath;
        }
    }
    public class GenericBEViewSettingItem
    {
        public Guid BusinessEntityDefinitionId { get; set; }
    }
}

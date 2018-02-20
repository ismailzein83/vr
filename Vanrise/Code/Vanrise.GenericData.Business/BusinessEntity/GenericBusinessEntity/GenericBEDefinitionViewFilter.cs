using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Business;

namespace Vanrise.GenericData.Business
{
    public class GenericBEDefinitionViewFilter : IBusinessEntityDefinitionFilter
    {
        public Guid ViewId { get; set; }
        public bool IsMatched(IBusinessEntityDefinitionFilterContext context)
        {
            ViewManager viewManager = new ViewManager();
            var userId = SecurityContext.Current.GetLoggedInUserId();

            var genericBEViewSettings = viewManager.GetView(this.ViewId).Settings as GenericBEViewSettings;

            if (genericBEViewSettings.Settings != null && !genericBEViewSettings.Settings.Any(x => x.BusinessEntityDefinitionId == context.entityDefinition.BusinessEntityDefinitionId))
                return false;

            bool haveViewAccess = new GenericBusinessEntityManager().DoesUserHaveViewAccess(userId, context.entityDefinition.BusinessEntityDefinitionId);
            if (!haveViewAccess)
                return false;
            return true;
        }
    }
}

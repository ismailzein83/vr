using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Business;

namespace Retail.BusinessEntity.Entities
{
    class AccountBEDefinitionFilter : IBusinessEntityDefinitionFilter
    {
        public Guid ViewId { get; set; }

        public bool IsMatched(IBusinessEntityDefinitionFilterContext context)
        {

            var settings = context.entityDefinition.Settings as AccountBEDefinitionSettings; 
            if (settings == null)
                return false;

            if (this.ViewId != Guid.Empty)
            {
                ViewManager viewManager = new ViewManager();
                var accountBEDefinitionViewSettings = viewManager.GetView(this.ViewId).Settings as AccountBEDefinitionViewSettings;
                BusinessEntityDefinitionManager businessEntityDefinitionManager = new BusinessEntityDefinitionManager();

                if (!accountBEDefinitionViewSettings.AccountBEDefinitionSettings.Any(x => x.BusinessEntityId == context.entityDefinition.BusinessEntityDefinitionId))
                    return false;
            }

            return true;
        }
    }
}

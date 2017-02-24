﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Business;

namespace Retail.BusinessEntity.Business
{
    public class AccountBEDefinitionViewFilter : IBusinessEntityDefinitionFilter
    {
        public Guid ViewId { get; set; }

        public bool IsMatched(IBusinessEntityDefinitionFilterContext context)
        {
            if (this.ViewId != Guid.Empty)
            {
                ViewManager viewManager = new ViewManager();
                var accountBEDefinitionViewSettings = viewManager.GetView(this.ViewId).Settings as AccountBEDefinitionViewSettings;
                AccountBEDefinitionManager accountBEDefinitionManager= new AccountBEDefinitionManager();

                if (!accountBEDefinitionViewSettings.Settings.Any(x => x.BusinessEntityId == context.entityDefinition.BusinessEntityDefinitionId))
                    return false;
                 
                if (!accountBEDefinitionManager.DoesUserHaveViewAccess(context.entityDefinition.BusinessEntityDefinitionId))
                    return false;
            }
            return true;
        }
    }
}

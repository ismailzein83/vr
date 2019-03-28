using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;

namespace Retail.BusinessEntity.MainExtensions.AccountBEActionTypes
{
    public class ListAccountActionSettings : AccountActionDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("32ABDAB1-2995-4BBE-8262-EA4D2045F932"); } }
        public override string ClientActionName { get { return "ListAccountAction"; } }
        public override bool DoesUserHaveAccess(IAccountActionDefinitionCheckAccessContext context)
        {
            return new AccountBEDefinitionManager().DoesUserHaveEditAccess(context.UserId, context.AccountBEDefinitionId);
        }
        public List<AccountActionDefinition> ActionDefinitions { get; set; }
    }
}

using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business
{
    public class DIDAccountFilter : IAccountFilter
    {
        Guid _accountDIDRelationDefinitionId = new ConfigManager().GetAccountDIDRelationDefinitionId();

        public bool IsExcluded(IAccountFilterContext context)
        {
            var beParentChildRelationDefinition = new BEParentChildRelationDefinitionManager().GetBEParentChildRelationDefinition(_accountDIDRelationDefinitionId);
            beParentChildRelationDefinition.ThrowIfNull("beParentChildRelationDefinition", _accountDIDRelationDefinitionId);
            beParentChildRelationDefinition.Settings.ThrowIfNull("beParentChildRelationDefinition.Settings", _accountDIDRelationDefinitionId);

            var accountBERuntimeSelectorFilter = beParentChildRelationDefinition.Settings.ParentBERuntimeSelectorFilter.CastWithValidate<AccountBERuntimeSelectorFilter>("beParentChildRelationDefinition.Settings.ParentBERuntimeSelectorFilter");
            var accountConditionAccountFilter = new AccountConditionAccountFilter() { AccountCondition = accountBERuntimeSelectorFilter.AccountCondition };

            return accountConditionAccountFilter.IsExcluded(context);
        }
    }
}

using System;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;

namespace Retail.Interconnect.Business
{
    public class InterConnectOperatorManager
    {
        public bool IsOperatorRepresentAsSwitch(long operatorId, Guid accountBEDefinitionId, Guid partDefinitionId)
        {
            var accountBEManager = new AccountBEManager();

            AccountPart accountPart;
            accountBEManager.TryGetAccountPart(accountBEDefinitionId, operatorId, partDefinitionId, false, out accountPart);

            if (accountPart != null && accountPart.Settings != null)
            {
                var accountPartSettings = accountPart.Settings as AccountPartInterconnectSetting;
                if (accountPartSettings != null)
                    return accountPartSettings.RepresentASwitch;
            }

            return false;
        }
    }
}

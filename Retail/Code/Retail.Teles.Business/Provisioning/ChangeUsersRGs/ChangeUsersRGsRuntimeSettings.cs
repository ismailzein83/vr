using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Common;

namespace Retail.Teles.Business
{
    public class ChangeUsersRGsRuntimeSettings : AccountProvisioner
    {
        AccountBEManager _accountBEManager = new AccountBEManager();
        ChangeUsersRGsManager _changeUsersRGsManager = new ChangeUsersRGsManager();

        public override void Execute(IAccountProvisioningContext context)
        {
            var definitionSettings = context.DefinitionSettings as ChangeUsersRGsDefinitionSettings;
            definitionSettings.ThrowIfNull("definitionSettings");
            var account = _accountBEManager.GetAccount(context.AccountBEDefinitionId, context.AccountId);
            account.ThrowIfNull("account", context.AccountId);
            context.WriteTrackingMessage(LogEntryType.Information, string.Format("Start loading users to {0}.", context.ActionDefinitionName));
            List<UsersToChangeRG> usersToChangeRG = _changeUsersRGsManager.GetUsersToChangeRG(context.AccountBEDefinitionId, account, definitionSettings.VRConnectionId, definitionSettings.CompanyTypeId, definitionSettings.SiteTypeId, definitionSettings.UserTypeId, definitionSettings.ExistingRoutingGroupCondition, definitionSettings.NewRoutingGroupCondition, definitionSettings.ExistingRGNoMatchHandling, definitionSettings.NewRGNoMatchHandling, definitionSettings.NewRGMultiMatchHandling);
            context.WriteTrackingMessage(LogEntryType.Information, string.Format("Start {0} users.", context.ActionDefinitionName));
            _changeUsersRGsManager.ChangeRGsAndUpdateState(usersToChangeRG, definitionSettings.ActionType, definitionSettings.VRConnectionId, definitionSettings.SaveChangesToAccountState, context.AccountBEDefinitionId,context);
        }
    }
}

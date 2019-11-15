using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Notification;
using Vanrise.Notification.Entities;

namespace Retail.Teles.Business
{
    public class ChangeUsersRGsAction : VRAction
    {
        AccountBEManager _accountBEManager = new AccountBEManager();
        ChangeUsersRGsManager _changeUsersRGsManager = new ChangeUsersRGsManager();

        public override void Execute(IVRActionExecutionContext context)
        {
            DataRecordAlertRuleActionEventPayload payload = context.EventPayload as DataRecordAlertRuleActionEventPayload;

            VRComponentTypeManager componentTypeManager = new VRComponentTypeManager();
            VRActionDefinition actionDefinition = componentTypeManager.GetComponentType<VRActionDefinitionSettings, VRActionDefinition>(DefinitionId);
            actionDefinition.Settings.ThrowIfNull("actionDefinition.Settings", DefinitionId);

            ChangeUsersRGsActionDefinition changeUsersRGsActionDefinition = actionDefinition.Settings.ExtendedSettings.CastWithValidate<ChangeUsersRGsActionDefinition>("actionDefinition.Settings.ExtendedSettings", DefinitionId);

            long accountId;
            var outputRecords = payload.OutputRecords;
            if (outputRecords != null && outputRecords.Count > 0)
            {
                accountId = (long)outputRecords.GetRecord("Company");
                var account = _accountBEManager.GetAccount(changeUsersRGsActionDefinition.AccountBEDefinitionId, accountId);
                account.ThrowIfNull("account", accountId);
                List<UsersToChangeRG> usersToChangeRG = _changeUsersRGsManager.GetUsersToChangeRG(changeUsersRGsActionDefinition.AccountBEDefinitionId, account, changeUsersRGsActionDefinition.VRConnectionId, changeUsersRGsActionDefinition.CompanyTypeId, changeUsersRGsActionDefinition.SiteTypeId, changeUsersRGsActionDefinition.UserTypeId, changeUsersRGsActionDefinition.ExistingRoutingGroupCondition, changeUsersRGsActionDefinition.NewRoutingGroupCondition, changeUsersRGsActionDefinition.ExistingRGNoMatchHandling, changeUsersRGsActionDefinition.NewRGNoMatchHandling, changeUsersRGsActionDefinition.NewRGMultiMatchHandling);
                _changeUsersRGsManager.ChangeRGsAndUpdateState(usersToChangeRG, changeUsersRGsActionDefinition.ActionType, changeUsersRGsActionDefinition.VRConnectionId, changeUsersRGsActionDefinition.SaveChangesToAccountState, changeUsersRGsActionDefinition.AccountBEDefinitionId);
            }            
        }
    }
}

using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.Teles.Business
{
    public class RevertUsersRGsRuntimeSettings : AccountProvisioner
    {
        TelesEnterpriseManager telesEnterpriseManager = new TelesEnterpriseManager();
        AccountBEManager accountBEManager = new AccountBEManager();
        public override void Execute(IAccountProvisioningContext context)
        {
            var definitionSettings = context.DefinitionSettings as RevertUsersRGsDefinitionSettings;
            if (definitionSettings == null)
                throw new NullReferenceException("definitionSettings");

            var accountExtendedSettings = new AccountBEManager().GetExtendedSettings<EnterpriseAccountMappingInfo>(context.AccountBEDefinitionId, context.AccountId);

            context.WriteTrackingMessage(LogEntryType.Information, string.Format("Loading Blocked Users."));

            var changeUsersRGsAccountState = new AccountBEManager().GetExtendedSettings<ChangeUsersRGsAccountState>(context.AccountBEDefinitionId, context.AccountId);
            ChURGsActionCh oldChURGsActionCh;
            var changedUsers = GetChangedUsers(context, definitionSettings.ActionType, definitionSettings.VRConnectionId, changeUsersRGsAccountState, out oldChURGsActionCh);
            context.WriteTrackingMessage(LogEntryType.Information, string.Format("Blocked Users Loaded."));

            RevertBlockedUsers(context,definitionSettings.VRConnectionId, changedUsers);
            RevertBlockedUsersState(context, changeUsersRGsAccountState, oldChURGsActionCh);
           
        }

        void RevertBlockedUsersState(IAccountProvisioningContext context, ChangeUsersRGsAccountState changeUsersRGsAccountState, ChURGsActionCh oldChURGsActionCh)
        {
            var currentUsersRGsAccountState = new AccountBEManager().GetExtendedSettings<ChangeUsersRGsAccountState>(context.AccountBEDefinitionId, context.AccountId);

            if (changeUsersRGsAccountState != null && changeUsersRGsAccountState.ChangesByActionType != null && changeUsersRGsAccountState.ChangesByActionType.Count == 0)
            {
                if (accountBEManager.DeleteAccountExtendedSetting<ChangeUsersRGsAccountState>(context.AccountBEDefinitionId, context.AccountId))
                {
                    context.TrackActionExecuted(null, oldChURGsActionCh);
                };
            }else
            {
                if (accountBEManager.UpdateAccountExtendedSetting<ChangeUsersRGsAccountState>(context.AccountBEDefinitionId, context.AccountId, changeUsersRGsAccountState))
                {
                    context.TrackActionExecuted(null, oldChURGsActionCh);
                };
            }
        }
        void RevertBlockedUsers(IAccountProvisioningContext context, Guid vrConnectionId, List<dynamic> changedUsers)
        {
            if (changedUsers != null)
            {
                foreach (dynamic changedUser in changedUsers)
                {
                    UpdateUser(vrConnectionId, changedUser);
                    context.WriteTrackingMessage(LogEntryType.Information, string.Format("User {0} Unblocked.", changedUser.loginName));
                }
            }
        }
        List<dynamic> GetChangedUsers(IAccountProvisioningContext context, string actionType, Guid vrConnectionId, ChangeUsersRGsAccountState changeUsersRGsAccountState, out ChURGsActionCh oldChURGsActionCh)
        {
            List<dynamic> changedUsers = null;
            oldChURGsActionCh = null;
            if (changeUsersRGsAccountState != null && changeUsersRGsAccountState.ChangesByActionType != null)
            {
                ChURGsActionCh chURGsActionCh;
              
                if (changeUsersRGsAccountState.ChangesByActionType.TryGetValue(actionType, out chURGsActionCh))
                {
                    if (chURGsActionCh.ChangesByUser != null)
                    {
                        changedUsers = new List<dynamic>();
                        foreach (var changesByUser in chURGsActionCh.ChangesByUser)
                        {
                            var user = GetUser(vrConnectionId, changesByUser.Key);
                            if(user != null)
                            {
                                context.WriteTrackingMessage(LogEntryType.Information, string.Format("User {0} Loaded.", user.loginName));
                                user.routingGroupId = changesByUser.Value.OriginalRGId;
                                changedUsers.Add(user);
                            }
                        }
                    }
                    oldChURGsActionCh = chURGsActionCh;
                    changeUsersRGsAccountState.ChangesByActionType.Remove(actionType);
                }
            }
            return changedUsers;
        }

        dynamic GetUser(Guid vrConnectionId, dynamic userId)
        {
            return telesEnterpriseManager.GetUser(vrConnectionId, userId);
        }
        void UpdateUser(Guid vrConnectionId, dynamic user)
        {
            telesEnterpriseManager.UpdateUser(vrConnectionId, user);
        }
    }
}

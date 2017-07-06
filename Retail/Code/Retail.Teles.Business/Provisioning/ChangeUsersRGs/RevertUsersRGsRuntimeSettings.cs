using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
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

            var account = accountBEManager.GetAccount(context.AccountBEDefinitionId, context.AccountId);

            if (!TelesAccountCondition.AllowRevertUserRGs(account, definitionSettings.CompanyTypeId, definitionSettings.SiteTypeId, definitionSettings.ActionType))
            {
                throw new Exception("Not Allow to Revert User Routing Groups");
            }
            context.WriteTrackingMessage(LogEntryType.Information, string.Format("Loading Users to Revert."));
            if (account.TypeId == definitionSettings.CompanyTypeId)// load changeUsersRGsAccountState from company
            {
                var accountChilds = accountBEManager.GetChildAccounts(context.AccountBEDefinitionId, context.AccountId, false);
                if (accountChilds != null)
                {
                    foreach (var child in accountChilds)
                    {
                        RevertSiteUsers(context, definitionSettings, child.AccountId);
                    }
                }
                RevertSiteUsers(context, definitionSettings, context.AccountId);

            }
            else if (account.TypeId == definitionSettings.SiteTypeId)// load changeUsersRGsAccountState from company
            {
                RevertSiteUsers(context, definitionSettings, context.AccountId);
            }
            context.WriteTrackingMessage(LogEntryType.Information, string.Format("Users Reverted Successfully."));
        }
        void RevertSiteUsers(IAccountProvisioningContext context, RevertUsersRGsDefinitionSettings definitionSettings,long accountId)
        {
            ChangeUsersRGsAccountState changeUsersRGsAccountState = new AccountBEManager().GetExtendedSettings<ChangeUsersRGsAccountState>(context.AccountBEDefinitionId, accountId);
            if(changeUsersRGsAccountState != null)
            {
                ChURGsActionCh oldChURGsActionCh = null;
                var changedUsers = GetChangedUsers(context, definitionSettings.ActionType, definitionSettings.VRConnectionId, changeUsersRGsAccountState, out oldChURGsActionCh);
                RevertBlockedUsers(context, definitionSettings.VRConnectionId, changedUsers);
                RevertBlockedUsersState(context, accountId, changeUsersRGsAccountState, oldChURGsActionCh);
            }
       
        }
        void RevertBlockedUsersState(IAccountProvisioningContext context,long accountId, ChangeUsersRGsAccountState changeUsersRGsAccountState, ChURGsActionCh oldChURGsActionCh)
        {
            var currentUsersRGsAccountState = new AccountBEManager().GetExtendedSettings<ChangeUsersRGsAccountState>(context.AccountBEDefinitionId, accountId);

            if (changeUsersRGsAccountState != null && changeUsersRGsAccountState.ChangesByActionType != null && changeUsersRGsAccountState.ChangesByActionType.Count == 0)
            {
                if (accountBEManager.DeleteAccountExtendedSetting<ChangeUsersRGsAccountState>(context.AccountBEDefinitionId, accountId))
                {
                    context.TrackActionExecuted(null, oldChURGsActionCh);
                };
            }else
            {
                if (accountBEManager.UpdateAccountExtendedSetting<ChangeUsersRGsAccountState>(context.AccountBEDefinitionId, accountId, changeUsersRGsAccountState))
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
                            if (user != null)
                            {
                                if (user.routingGroupId == changesByUser.Value.ChangedRGId)
                                {
                                    context.WriteTrackingMessage(LogEntryType.Information, string.Format("User {0} Loaded.", user.loginName));
                                    user.routingGroupId = changesByUser.Value.OriginalRGId;
                                    changedUsers.Add(user);
                                }
                                else
                                {
                                    context.WriteTrackingMessage(LogEntryType.Information, string.Format("Connot revert routing group of user {0} due to external change.", user.loginName));
                                }
                            }
                        }
                    }
                    oldChURGsActionCh = chURGsActionCh;
                    changeUsersRGsAccountState.ChangesByActionType.Remove(actionType);
                }
            }
            return changedUsers;
        }

        dynamic GetUser(Guid vrConnectionId, string userId)
        {
            return telesEnterpriseManager.GetUser(vrConnectionId, userId);
        }
        void UpdateUser(Guid vrConnectionId, dynamic user)
        {
            telesEnterpriseManager.UpdateUser(vrConnectionId, user);
        }
    }
}

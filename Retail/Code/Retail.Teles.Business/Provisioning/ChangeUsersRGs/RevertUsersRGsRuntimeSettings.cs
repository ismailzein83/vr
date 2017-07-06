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
            string accountName = accountBEManager.GetAccountName(account);
            if (!TelesAccountCondition.AllowRevertUserRGs(account, definitionSettings.CompanyTypeId, definitionSettings.SiteTypeId, definitionSettings.ActionType))
            {
                throw new Exception(string.Format("Not Allow to {0} User Routing Groups", context.ActionDefinitionName));
            }
            if (account.TypeId == definitionSettings.CompanyTypeId)// load changeUsersRGsAccountState from company
            {
                context.WriteTrackingMessage(LogEntryType.Information, "Loading Sites to {0}.", context.ActionDefinitionName);
                var accountChilds = accountBEManager.GetChildAccounts(context.AccountBEDefinitionId, context.AccountId, false);
                if (accountChilds != null)
                {
                    foreach (var child in accountChilds)
                    {
                        string childAccountName = accountBEManager.GetAccountName(child);
                        context.WriteTrackingMessage(LogEntryType.Information, "Start {0} for Site: {1}.", context.ActionDefinitionName, childAccountName);
                        RevertSiteUsers(context, definitionSettings, child, childAccountName);
                        context.WriteTrackingMessage(LogEntryType.Information,"Finish {0} for Site: {1}.",context.ActionDefinitionName, childAccountName);

                    }
                }
                RevertSiteUsers(context, definitionSettings, account, accountName);

            }
            else if (account.TypeId == definitionSettings.SiteTypeId)// load changeUsersRGsAccountState from company
            {
                context.WriteTrackingMessage(LogEntryType.Information, "Start {0} for Site: {0}.", context.ActionDefinitionName, accountName);
                RevertSiteUsers(context, definitionSettings, account, accountName);
                context.WriteTrackingMessage(LogEntryType.Information, "Finish {0} for Site: {0}.", context.ActionDefinitionName, accountName);
            }
        }
        void RevertSiteUsers(IAccountProvisioningContext context, RevertUsersRGsDefinitionSettings definitionSettings,Account account, string accountname)
        {
            ChangeUsersRGsAccountState changeUsersRGsAccountState = accountBEManager.GetExtendedSettings<ChangeUsersRGsAccountState>(context.AccountBEDefinitionId, account.AccountId);
            if(changeUsersRGsAccountState != null)
            {
                ChURGsActionCh oldChURGsActionCh = null;
                var changedUsers = GetChangedUsers(context, definitionSettings.ActionType, definitionSettings.VRConnectionId, changeUsersRGsAccountState, out oldChURGsActionCh);
                RevertUsers(context, definitionSettings.VRConnectionId, changedUsers);
                RevertUsersState(context, account.AccountId, changeUsersRGsAccountState, oldChURGsActionCh);
                if (changedUsers != null && changedUsers.Count > 0)
                    context.WriteTrackingMessage(LogEntryType.Information, "Users {0} Successfully for Site: {1}.", context.ActionDefinitionName, accountname);
                else
                    context.WriteTrackingMessage(LogEntryType.Information, "Site: {0} has no Users to {1}.", accountname, context.ActionDefinitionName);
            }
       
        }
        void RevertUsersState(IAccountProvisioningContext context,long accountId, ChangeUsersRGsAccountState changeUsersRGsAccountState, ChURGsActionCh oldChURGsActionCh)
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
        void RevertUsers(IAccountProvisioningContext context, Guid vrConnectionId, List<dynamic> changedUsers)
        {
            if (changedUsers != null)
            {
                List<string> usersNames = new List<string>();

                foreach (dynamic changedUser in changedUsers)
                {
                    usersNames.Add(changedUser.loginName);
                    UpdateUser(vrConnectionId, changedUser);
                    if (usersNames.Count == 10)
                    {
                        WriteUsersProccessedTrackingMessage(context, usersNames);
                        usersNames = new List<string>();
                    }
                }
                WriteUsersProccessedTrackingMessage(context, usersNames);
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
        private void WriteUsersProccessedTrackingMessage(IAccountProvisioningContext context, List<string> usersNames)
        {
            if (usersNames.Count > 0)
            {
                context.WriteTrackingMessage(LogEntryType.Information, string.Format("{0} for Users: {1}. ",context.ActionDefinitionName, String.Join<string>(",", usersNames)));
            }
        }

    }
}

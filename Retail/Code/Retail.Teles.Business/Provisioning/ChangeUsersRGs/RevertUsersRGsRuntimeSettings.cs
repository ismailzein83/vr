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
        TelesUserManager _telesUserManager = new TelesUserManager();
        AccountBEManager _accountBEManager = new AccountBEManager();
        public override void Execute(IAccountProvisioningContext context)
        {
            var definitionSettings = context.DefinitionSettings as RevertUsersRGsDefinitionSettings;
            definitionSettings.ThrowIfNull("definitionSettings");
            var account = _accountBEManager.GetAccount(context.AccountBEDefinitionId, context.AccountId);
            account.ThrowIfNull("account", context.AccountId);
            context.WriteTrackingMessage(LogEntryType.Information, string.Format("Start loading users to {0}.",context.ActionDefinitionName));
            List<UsersToRevertRG> usersToRevertRG = GetUsersToRevertRG(context, account, definitionSettings);
            context.WriteTrackingMessage(LogEntryType.Information, string.Format("Start {0} users.", context.ActionDefinitionName));
            RevertRGsAndUpdateState(context, usersToRevertRG, definitionSettings);
        }
        private List<UsersToRevertRG> GetUsersToRevertRG(IAccountProvisioningContext context, Account account, RevertUsersRGsDefinitionSettings definitionSettings)
        {
            List<UsersToRevertRG> usersToRevertRG = new List<UsersToRevertRG>();
            AddUsersToRevertFromAccountState(context, account, usersToRevertRG, definitionSettings);
            List<Account> childAccounts = _accountBEManager.GetChildAccounts(context.AccountBEDefinitionId, account.AccountId, true);
            if (childAccounts != null)
            {
                foreach (var child in childAccounts)
                {
                    AddUsersToRevertFromAccountState(context, child, usersToRevertRG, definitionSettings);
                }
            }
            return usersToRevertRG;
        }
        private void AddUsersToRevertFromAccountState(IAccountProvisioningContext context,Account account, List<UsersToRevertRG> usersToRevertRG, RevertUsersRGsDefinitionSettings definitionSettings)
        {
            ChangeUsersRGsAccountState changeUsersRGsAccountState = _accountBEManager.GetExtendedSettings<ChangeUsersRGsAccountState>(context.AccountBEDefinitionId, account.AccountId);
            if (changeUsersRGsAccountState != null && changeUsersRGsAccountState.ChangesByActionType != null)
            {
                ChURGsActionCh chURGsActionCh;
                if (changeUsersRGsAccountState.ChangesByActionType.TryGetValue(definitionSettings.ActionType, out chURGsActionCh))
                {
                    UsersToRevertRG currentUsersToRevertRG = new UsersToRevertRG(account)
                    {
                        TelesUsers = new List<TelesUser>()
                    };
                    if (chURGsActionCh.ChangesByUser != null)
                    {
                        foreach (var user in chURGsActionCh.ChangesByUser)
                        {
                            var telesUser = GetTelesUser(definitionSettings.VRConnectionId, user.Key);
                            if (telesUser != null)
                            {
                                if (telesUser.routingGroupId == user.Value.ChangedRGId)
                                {
                                    telesUser.routingGroupId = user.Value.OriginalRGId;
                                    currentUsersToRevertRG.TelesUsers.Add(new TelesUser
                                    {
                                        User = telesUser,
                                        UserId = user.Key
                                    });
                                }
                            }
                        } 
                    }
                    usersToRevertRG.Add(currentUsersToRevertRG);
                }
            }
        }
        private void RevertRGsAndUpdateState(IAccountProvisioningContext context, List<UsersToRevertRG> usersToRevertRG, RevertUsersRGsDefinitionSettings definitionSettings)
        {
            if (usersToRevertRG != null && usersToRevertRG.Count > 0)
            {
                foreach (var userToRevert in usersToRevertRG)
                {
                    ChURGsActionCh oldChURGsActionCh = userToRevert.ExistingAccountState.ChangesByActionType.GetRecord(definitionSettings.ActionType);
                    ChangeUsersRGsAccountState existingAccountState = userToRevert.ExistingAccountState.VRDeepCopy();
                    ChURGsActionCh chURGsActionCh = existingAccountState.ChangesByActionType.GetRecord(definitionSettings.ActionType);
                    if (chURGsActionCh == null)
                        continue;
                    if (userToRevert.TelesUsers != null)
                    {
                       
                        List<string> usersNames = new List<string>();
                       
                        foreach (var user in userToRevert.TelesUsers)
                        {

                            UpdateTelesUser(definitionSettings.VRConnectionId, user.User);
                            usersNames.Add(user.User.loginName.Value);
                            if (usersNames.Count == 10)
                            {
                                WriteUsersProccessedTrackingMessage(context, usersNames);
                                usersNames = new List<string>();
                            }
                            WriteUsersProccessedTrackingMessage(context, usersNames);
                            chURGsActionCh.ChangesByUser.Remove(user.UserId);
                        }
                    }
                    if (chURGsActionCh.ChangesByUser == null || chURGsActionCh.ChangesByUser.Count == 0)
                        existingAccountState.ChangesByActionType.Remove(definitionSettings.ActionType);
                    if (existingAccountState.ChangesByActionType.Count == 0)
                    {
                        if (_accountBEManager.DeleteAccountExtendedSetting<ChangeUsersRGsAccountState>(context.AccountBEDefinitionId, userToRevert.Account.AccountId))
                        {
                            context.TrackActionExecuted(userToRevert.Account.AccountId,null, oldChURGsActionCh);
                        };
                    }
                    else
                    {
                        if (_accountBEManager.UpdateAccountExtendedSetting<ChangeUsersRGsAccountState>(context.AccountBEDefinitionId, userToRevert.Account.AccountId, existingAccountState))
                        {
                            context.TrackActionExecuted(userToRevert.Account.AccountId,null, oldChURGsActionCh);
                        };
                    }
                    if (userToRevert.TelesUsers.Count > 0)
                        context.WriteTrackingMessage(LogEntryType.Information, "Users {0} successfully for account: {1}.", context.ActionDefinitionName, _accountBEManager.GetAccountName(userToRevert.Account));
                    else
                        context.WriteTrackingMessage(LogEntryType.Information, "Account: {0} has no users to {1}.", _accountBEManager.GetAccountName(userToRevert.Account), context.ActionDefinitionName);
                }
            }
        }
        dynamic GetTelesUser(Guid vrConnectionId, string userId)
        {
            return _telesUserManager.GetUser(vrConnectionId, userId);
        }
        void UpdateTelesUser(Guid vrConnectionId, dynamic user)
        {
            _telesUserManager.UpdateUser(vrConnectionId, user);
        }
        private void WriteUsersProccessedTrackingMessage(IAccountProvisioningContext context, List<string> usersNames)
        {
            if (usersNames.Count > 0)
            {
                context.WriteTrackingMessage(LogEntryType.Information, string.Format("{0} for Users: {1}. ",context.ActionDefinitionName, String.Join<string>(",", usersNames)));
            }
        }

        private class UsersToRevertRG
        {
            Account _account;
            ChangeUsersRGsAccountState _existingAccountState;
            static AccountBEManager s_accountBEManager = new AccountBEManager();
            public UsersToRevertRG(Account account)
            {
                _account = account;
                _existingAccountState = s_accountBEManager.GetExtendedSettings<ChangeUsersRGsAccountState>(account);
            }
            public List<TelesUser> TelesUsers { get; set; }

            public Account Account
            {
                get
                {
                    return _account;
                }
            }

            public ChangeUsersRGsAccountState ExistingAccountState
            {
                get
                {
                    return _existingAccountState;
                }
            }
        }
        private class TelesUser
        {
            public string UserId { get; set; }
            public dynamic User { get; set; }
        }
    }
}

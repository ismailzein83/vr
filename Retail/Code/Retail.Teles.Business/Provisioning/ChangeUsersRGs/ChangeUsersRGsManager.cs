using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Teles.Business.Provisioning;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Entities;

namespace Retail.Teles.Business
{
    public class ChangeUsersRGsManager
    {
        AccountBEManager _accountBEManager = new AccountBEManager();
        TelesEnterpriseManager _telesEnterpriseManager = new TelesEnterpriseManager();
        TelesSiteManager _telesSiteManager = new TelesSiteManager();
        TelesUserManager _telesUserManager = new TelesUserManager();

        #region Public Methods
        public List<UsersToChangeRG> GetUsersToChangeRG(Guid accountBEDefinitionId, Account account, Guid vrConnectionId, Guid companyTypeId, Guid siteTypeId, Guid? userTypeId, RoutingGroupCondition existingRoutingGroupCondition, RoutingGroupCondition newRoutingGroupCondition, ExistingRGNoMatchHandling existingRGNoMatchHandling, NewRGNoMatchHandling newRGNoMatchHandling, NewRGMultiMatchHandling newRGMultiMatchHandling)
        {
            List<UsersToChangeRG> usersToChangeRG = new List<UsersToChangeRG>();
            if (account.TypeId == companyTypeId)
            {
                AddUsersToChangeRGfromCompany(accountBEDefinitionId, account, usersToChangeRG, vrConnectionId, siteTypeId, userTypeId, existingRoutingGroupCondition, newRoutingGroupCondition, existingRGNoMatchHandling, newRGNoMatchHandling, newRGMultiMatchHandling);
            }

            else if (account.TypeId == siteTypeId)
            {
                EnterpriseAccountMappingInfo enterpriseAccountMappingInfo = null;
                if (account.ParentAccountId.HasValue)
                {
                    var parentCompanyAccount = _accountBEManager.GetSelfOrParentAccountOfType(accountBEDefinitionId, account.ParentAccountId.Value, companyTypeId);
                    if (parentCompanyAccount != null)
                        enterpriseAccountMappingInfo = _accountBEManager.GetExtendedSettings<EnterpriseAccountMappingInfo>(parentCompanyAccount);
                }
                string mappedTelesSiteId;
                AddUsersToChangeRGfromSite(accountBEDefinitionId, account, usersToChangeRG, vrConnectionId, userTypeId, existingRoutingGroupCondition, newRoutingGroupCondition, existingRGNoMatchHandling, newRGNoMatchHandling, newRGMultiMatchHandling, out mappedTelesSiteId);
            }
            else if (userTypeId.HasValue && userTypeId.Value == account.TypeId)
            {
                SiteAccountMappingInfo siteAccountMappingInfo = null;
                if (account.ParentAccountId.HasValue)
                {
                    var parentSiteAccount = _accountBEManager.GetSelfOrParentAccountOfType(accountBEDefinitionId, account.ParentAccountId.Value, siteTypeId);
                    if (parentSiteAccount != null)
                        siteAccountMappingInfo = _accountBEManager.GetExtendedSettings<SiteAccountMappingInfo>(parentSiteAccount);
                }
                string mappedTelesUserId;
                AddUsersToChangeRGfromUser(account, siteAccountMappingInfo, usersToChangeRG, vrConnectionId, existingRoutingGroupCondition, newRoutingGroupCondition, existingRGNoMatchHandling, newRGNoMatchHandling, newRGMultiMatchHandling, out mappedTelesUserId);
            }
            return usersToChangeRG;
        }
        public void ChangeRGsAndUpdateState(List<UsersToChangeRG> usersToChangeRG, string actionType, Guid vrConnectionId, bool saveChangesToAccountState, Guid accountBEDefinitionId)
        {
            ChangeRGsAndUpdateState(usersToChangeRG, actionType, vrConnectionId, saveChangesToAccountState, accountBEDefinitionId, null);
        }
        public void ChangeRGsAndUpdateState(List<UsersToChangeRG> usersToChangeRG, string actionType, Guid vrConnectionId, bool saveChangesToAccountState, Guid accountBEDefinitionId, IAccountProvisioningContext context)
        {
            if (usersToChangeRG != null)
            {
                foreach (var userToChange in usersToChangeRG)
                {

                    if (context != null)
                        context.WriteTrackingMessage(LogEntryType.Information, string.Format("Start processing account: {0}.", _accountBEManager.GetAccountName(userToChange.Account)));
                    ChangeUsersRGsAccountState existingAccountState = null;
                    if (userToChange.ExistingAccountState != null)
                        existingAccountState = userToChange.ExistingAccountState.VRDeepCopy();
                    else
                    {
                        existingAccountState = new ChangeUsersRGsAccountState();
                    }
                    if (existingAccountState.ChangesByActionType == null)
                        existingAccountState.ChangesByActionType = new Dictionary<string, ChURGsActionCh>();
                    ChURGsActionCh chURGsActionCh = existingAccountState.ChangesByActionType.GetOrCreateItem(actionType);
                    if (userToChange.TelesUsers != null)
                    {
                        List<string> usersNames = new List<string>();
                        if (chURGsActionCh.ChangesByUser == null)
                            chURGsActionCh.ChangesByUser = new Dictionary<string, ChURGsUserCh>();

                        foreach (var user in userToChange.TelesUsers)
                        {
                            UpdateUser(vrConnectionId, user.User);
                            usersNames.Add(user.User.loginName.Value);
                            if (usersNames.Count == 10)
                            {
                                if (context != null)
                                    WriteUsersProccessedTrackingMessage(context, usersNames);
                                usersNames = new List<string>();
                            }
                            if (saveChangesToAccountState)
                            {
                                ChURGsUserCh chURGsUserCh = chURGsActionCh.ChangesByUser.GetOrCreateItem(user.UserId);
                                chURGsUserCh.ChangedRGId = user.NewRoutingGroupId;
                                chURGsUserCh.OriginalRGId = user.OldRoutingGroupId;
                                chURGsUserCh.SiteId = user.SiteId;

                            }
                        }
                        if (context != null)
                            WriteUsersProccessedTrackingMessage(context, usersNames);

                    }
                    if (saveChangesToAccountState)
                    {
                        if (_accountBEManager.UpdateAccountExtendedSetting<ChangeUsersRGsAccountState>(accountBEDefinitionId, userToChange.Account.AccountId, existingAccountState))
                        {
                            if (context != null)
                                context.TrackActionExecuted(userToChange.Account.AccountId, null, existingAccountState);
                        };
                    }
                    if (context != null)
                        context.WriteTrackingMessage(LogEntryType.Information, string.Format("Finish processing account: {0}.", _accountBEManager.GetAccountName(userToChange.Account)));

                }
            }
        }

        #endregion

        #region Private Methods
        private void AddUsersToChangeRGfromCompany(Guid accountBEDefinitionId, Account companyAccount, List<UsersToChangeRG> usersToChangeRG, Guid vrConnectionId, Guid siteTypeId, Guid? userTypeId, RoutingGroupCondition existingRoutingGroupCondition, RoutingGroupCondition newRoutingGroupCondition, ExistingRGNoMatchHandling existingRGNoMatchHandling, NewRGNoMatchHandling newRGNoMatchHandling, NewRGMultiMatchHandling newRGMultiMatchHandling)
        {
            EnterpriseAccountMappingInfo enterpriseAccountMappingInfo = _accountBEManager.GetExtendedSettings<EnterpriseAccountMappingInfo>(companyAccount);
            Dictionary<string, dynamic> telesSites = null;
            if (enterpriseAccountMappingInfo != null)
                telesSites = GetTelesSites(vrConnectionId, enterpriseAccountMappingInfo.TelesEnterpriseId);
            List<Account> childAccounts = _accountBEManager.GetChildAccounts(accountBEDefinitionId, companyAccount.AccountId, true);
            List<string> mappedTelesSiteIds = new List<string>();
            if (childAccounts != null)
            {
                foreach (var child in childAccounts)
                {
                    if (child.TypeId == siteTypeId)
                    {
                        string telesSiteId;
                        AddUsersToChangeRGfromSite(accountBEDefinitionId, child, usersToChangeRG, vrConnectionId, userTypeId, existingRoutingGroupCondition, newRoutingGroupCondition, existingRGNoMatchHandling, newRGNoMatchHandling, newRGMultiMatchHandling, out telesSiteId);
                        if (telesSiteId != null)
                            mappedTelesSiteIds.Add(telesSiteId);
                    }
                }
            }
            List<TelesUser> telesUsersToChangeRG = null;

            if (telesSites != null)
            {
                telesUsersToChangeRG = new List<TelesUser>();
                foreach (var telesSiteEntry in telesSites)
                {
                    if (mappedTelesSiteIds.Contains(telesSiteEntry.Key))//teles site is mapped to Retail Account of type site and should be already executed in previous block of code
                        continue;
                    var siteTelesUsersToChangeRG = GetTelesUsersToChangeRGFromTelesSite(telesSiteEntry.Key, null, vrConnectionId, existingRoutingGroupCondition, newRoutingGroupCondition, existingRGNoMatchHandling, newRGNoMatchHandling, newRGMultiMatchHandling);
                    if (siteTelesUsersToChangeRG != null && siteTelesUsersToChangeRG.Count > 0)
                        telesUsersToChangeRG.AddRange(siteTelesUsersToChangeRG);
                }
            }
            usersToChangeRG.Add(new UsersToChangeRG(companyAccount)
            {
                TelesUsers = telesUsersToChangeRG
            });

        }

        private void AddUsersToChangeRGfromSite(Guid accountBEDefinitionId, Account siteAccount, List<UsersToChangeRG> usersToChangeRG, Guid vrConnectionId, Guid? userTypeId, RoutingGroupCondition existingRoutingGroupCondition, RoutingGroupCondition newRoutingGroupCondition, ExistingRGNoMatchHandling existingRGNoMatchHandling, NewRGNoMatchHandling newRGNoMatchHandling, NewRGMultiMatchHandling newRGMultiMatchHandling, out string telesSiteId)
        {
            SiteAccountMappingInfo siteAccountMappingInfo = _accountBEManager.GetExtendedSettings<SiteAccountMappingInfo>(siteAccount);

            List<string> mappedTelesUserIds = new List<string>();
            if (userTypeId.HasValue)
            {
                List<Account> childAccounts = _accountBEManager.GetChildAccounts(accountBEDefinitionId, siteAccount.AccountId, true);
                if (childAccounts != null)
                {
                    foreach (var child in childAccounts)
                    {
                        if (child.TypeId == userTypeId.Value)
                        {
                            string mappedTelesUserId;
                            AddUsersToChangeRGfromUser(child, siteAccountMappingInfo, usersToChangeRG, vrConnectionId, existingRoutingGroupCondition, newRoutingGroupCondition, existingRGNoMatchHandling, newRGNoMatchHandling, newRGMultiMatchHandling, out mappedTelesUserId);
                            if (mappedTelesUserId != null)
                                mappedTelesUserIds.Add(mappedTelesUserId);
                        }
                    }
                }
            }
            List<TelesUser> telesUsersToChangeRG = null;

            if (siteAccountMappingInfo != null)
            {
                telesUsersToChangeRG = new List<TelesUser>();

                telesSiteId = siteAccountMappingInfo.TelesSiteId;
                telesSiteId.ThrowIfNull("telesSiteId");
                telesUsersToChangeRG = GetTelesUsersToChangeRGFromTelesSite(telesSiteId, mappedTelesUserIds, vrConnectionId, existingRoutingGroupCondition, newRoutingGroupCondition, existingRGNoMatchHandling, newRGNoMatchHandling, newRGMultiMatchHandling);
            }
            else
            {
                telesSiteId = null;
            }
            usersToChangeRG.Add(new UsersToChangeRG(siteAccount)
            {
                TelesUsers = telesUsersToChangeRG
            });

        }

        private void AddUsersToChangeRGfromUser(Account userAccount, SiteAccountMappingInfo siteAccountMappingInfo, List<UsersToChangeRG> usersToChangeRG, Guid vrConnectionId, RoutingGroupCondition existingRoutingGroupCondition, RoutingGroupCondition newRoutingGroupCondition, ExistingRGNoMatchHandling existingRGNoMatchHandling, NewRGNoMatchHandling newRGNoMatchHandling, NewRGMultiMatchHandling newRGMultiMatchHandling, out string mappedTelesUserId)
        {
            string telesSiteId = null;
            if (siteAccountMappingInfo != null)
                telesSiteId = siteAccountMappingInfo.TelesSiteId;

            UserAccountMappingInfo userAccountMappingInfo = _accountBEManager.GetExtendedSettings<UserAccountMappingInfo>(userAccount);
            mappedTelesUserId = null;
            if (userAccountMappingInfo != null)
            {
                mappedTelesUserId = userAccountMappingInfo.TelesUserId;
                if (userAccountMappingInfo.TelesSiteId != null)
                    telesSiteId = userAccountMappingInfo.TelesSiteId;

                telesSiteId.ThrowIfNull("telesSiteId");

                List<string> existingRoutingGroups;
                string newRoutingGroup;
                bool? shouldUpdate;
                GetTelesSiteRoutingGroups(telesSiteId, vrConnectionId, existingRoutingGroupCondition, newRoutingGroupCondition, existingRGNoMatchHandling, newRGNoMatchHandling, newRGMultiMatchHandling, out existingRoutingGroups, out newRoutingGroup, out shouldUpdate);
                var currentTelesUser = GetTelesUser(vrConnectionId, mappedTelesUserId);

                TelesUser telesUser;
                var usersToChange = new UsersToChangeRG(userAccount);
                if (ShouldChangeTelesUserRG(mappedTelesUserId, currentTelesUser, telesSiteId, existingRoutingGroups, newRoutingGroup, shouldUpdate, out telesUser))
                {
                    usersToChange.TelesUsers = new List<TelesUser> { telesUser };

                }
                usersToChangeRG.Add(usersToChange);
            }

        }

        private List<TelesUser> GetTelesUsersToChangeRGFromTelesSite(string telesSiteId, List<string> excludedUserIds, Guid vrConnectionId, RoutingGroupCondition existingRoutingGroupCondition, RoutingGroupCondition newRoutingGroupCondition, ExistingRGNoMatchHandling existingRGNoMatchHandling, NewRGNoMatchHandling newRGNoMatchHandling, NewRGMultiMatchHandling newRGMultiMatchHandling)
        {
            List<TelesUser> telesUsersToChangeRG = new List<TelesUser>();
            Dictionary<string, dynamic> telesSiteUsers = GetTelesSiteUsers(vrConnectionId, telesSiteId);
            if (telesSiteUsers != null && telesSiteUsers.Count > 0)
            {
                List<string> existingRoutingGroups;
                string newRoutingGroup;
                bool? shouldUpdate;
                GetTelesSiteRoutingGroups(telesSiteId, vrConnectionId, existingRoutingGroupCondition, newRoutingGroupCondition, existingRGNoMatchHandling, newRGNoMatchHandling, newRGMultiMatchHandling, out existingRoutingGroups, out newRoutingGroup, out shouldUpdate);
                foreach (var telesUserEntry in telesSiteUsers)
                {
                    if (excludedUserIds != null && excludedUserIds.Contains(telesUserEntry.Key))
                        continue;
                    TelesUser telesUser;
                    if (ShouldChangeTelesUserRG(telesUserEntry.Key, telesUserEntry.Value, telesSiteId, existingRoutingGroups, newRoutingGroup, shouldUpdate, out telesUser))
                        telesUsersToChangeRG.Add(telesUser);
                }
            }
            return telesUsersToChangeRG;
        }

        private void GetTelesSiteRoutingGroups(string telesSiteId, Guid vrConnectionId, RoutingGroupCondition existingRoutingGroupCondition, RoutingGroupCondition newRoutingGroupCondition, ExistingRGNoMatchHandling existingRGNoMatchHandling, NewRGNoMatchHandling newRGNoMatchHandling, NewRGMultiMatchHandling newRGMultiMatchHandling, out List<string> existingRoutingGroups, out string newRoutingGroup, out bool? shouldUpdate)
        {
            shouldUpdate = null;
            Dictionary<string, dynamic> siteRoutingGroups = GetSiteRoutingGroups(vrConnectionId, telesSiteId);
            existingRoutingGroups = null;
            newRoutingGroup = null;
            if (siteRoutingGroups != null)
            {
                foreach (dynamic siteRoutingGroup in siteRoutingGroups.Values)
                {
                    string siteRoutingGroupId = siteRoutingGroup.id.ToString();
                    if (existingRoutingGroupCondition != null)
                    {
                        if (existingRoutingGroups == null)
                            existingRoutingGroups = new List<string>();
                        RoutingGroupConditionContext oldcontext = new RoutingGroupConditionContext
                        {
                            RoutingGroupName = siteRoutingGroup.name
                        };
                        if (existingRoutingGroupCondition.Evaluate(oldcontext))
                        {
                            existingRoutingGroups.Add(siteRoutingGroupId);
                        }
                    }
                    RoutingGroupConditionContext newcontext = new RoutingGroupConditionContext
                    {
                        RoutingGroupName = siteRoutingGroup.name
                    };
                    if (newRoutingGroupCondition.Evaluate(newcontext))
                    {
                        if (newRoutingGroup != null)
                        {
                            switch (newRGMultiMatchHandling)
                            {
                                case NewRGMultiMatchHandling.Skip:
                                    shouldUpdate = false;
                                    break;
                                case NewRGMultiMatchHandling.Stop: throw new Exception("More than one routing group available for new routing group condition.");
                            }
                        }
                        newRoutingGroup = siteRoutingGroupId;
                    }
                }
            }

            if (newRoutingGroup == null)
            {
                switch (newRGNoMatchHandling)
                {
                    case NewRGNoMatchHandling.Skip:
                        shouldUpdate = false;
                        break;
                    case NewRGNoMatchHandling.Stop: throw new Exception("No routing group available for new routing group condition.");
                }
            }
            if (existingRoutingGroupCondition == null)
            {
                shouldUpdate = true;
            }
            else if (existingRoutingGroups == null || existingRoutingGroups.Count == 0)
            {
                switch (existingRGNoMatchHandling)
                {
                    case ExistingRGNoMatchHandling.Skip:
                        shouldUpdate = false;
                        break;
                    case ExistingRGNoMatchHandling.UpdateAll:
                        shouldUpdate = true;
                        break;
                    case ExistingRGNoMatchHandling.Stop:
                        throw new Exception("No routing group available for existing routing group condition.");
                }
            }
        }

        private bool ShouldChangeTelesUserRG(string telesUserId, dynamic telesUserObject, string telesSiteId, List<string> existingRoutingGroups, string newRoutingGroup, bool? shouldUpdate, out TelesUser telesUser)
        {
            telesUser = null;
            if (telesUserObject != null)
            {

                if (telesUserObject.routingGroupId != newRoutingGroup && ((shouldUpdate.HasValue && shouldUpdate.Value) || (existingRoutingGroups != null && existingRoutingGroups.Contains(telesUserObject.routingGroupId.ToString()))))
                {
                    telesUser = new TelesUser
                    {
                        UserId = telesUserId,
                        SiteId = telesSiteId,
                        NewRoutingGroupId = newRoutingGroup,
                        OldRoutingGroupId = telesUserObject.routingGroupId
                    };
                    telesUserObject.routingGroupId = newRoutingGroup;
                    telesUser.User = telesUserObject;
                    return true;
                }
            }
            return false;

        }

        private void UpdateUser(Guid vrConnectionId, dynamic user)
        {
            _telesUserManager.UpdateUser(vrConnectionId, user);
        }

        private void WriteUsersProccessedTrackingMessage(IAccountProvisioningContext context, List<string> usersNames)
        {
            if (usersNames.Count > 0)
            {
                if (context != null)
                    context.WriteTrackingMessage(LogEntryType.Information, string.Format("Users processed: {0}. ", String.Join<string>(",", usersNames)));
            }
        }

        private Dictionary<string, dynamic> GetSiteRoutingGroups(Guid vrConnectionId, string siteId)
        {
            return _telesSiteManager.GetSiteRoutingGroups(vrConnectionId, siteId);
        }

        private Dictionary<string, dynamic> GetTelesSiteUsers(Guid vrConnectionId, string siteId)
        {
            IEnumerable<dynamic> users = _telesUserManager.GetUsers(vrConnectionId, siteId);
            Dictionary<string, dynamic> dict = new Dictionary<string, dynamic>();
            if (users != null)
            {
                foreach (var u in users)
                {
                    string userId = u.id.Value.ToString();
                    if (dict.ContainsKey(userId))
                        throw new Exception(String.Format("User Id '{0}' already exists for site Id '{1}'", userId, siteId));
                    dict.Add(userId, u);
                }
            }
            return dict;
        }

        private Dictionary<string, dynamic> GetTelesSites(Guid vrConnectionId, string telesEnterpriseId)
        {
            IEnumerable<dynamic> sites = _telesSiteManager.GetSites(vrConnectionId, telesEnterpriseId);
            Dictionary<string, dynamic> dict = new Dictionary<string, dynamic>();
            if (sites != null)
            {
                foreach (var s in sites)
                {
                    string siteId = s.id.Value.ToString();
                    if (dict.ContainsKey(siteId))
                        throw new Exception(String.Format("Site Id '{0}' already exists for enterprise Id '{1}'", siteId, telesEnterpriseId));
                    dict.Add(siteId, s);
                }
            }
            return dict;
        }

        private dynamic GetTelesUser(Guid vrConnectionId, string telesSiteId)
        {
            return _telesUserManager.GetUser(vrConnectionId, telesSiteId);
        }

        #endregion
    }

    public class TelesUser
    {
        public string UserId { get; set; }

        public dynamic User { get; set; }

        public string SiteId { get; set; }

        public string OldRoutingGroupId { get; set; }

        public string NewRoutingGroupId { get; set; }
    }

    public class UsersToChangeRG
    {
        Account _account;
        ChangeUsersRGsAccountState _existingAccountState;
        static AccountBEManager s_accountBEManager = new AccountBEManager();
        public UsersToChangeRG(Account account)
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

}

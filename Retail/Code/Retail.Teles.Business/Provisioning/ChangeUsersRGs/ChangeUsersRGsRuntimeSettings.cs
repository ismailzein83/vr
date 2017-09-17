using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Teles.Business.Provisioning;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Integration.Entities;
using Vanrise.Common;

namespace Retail.Teles.Business
{
    public class ChangeUsersRGsRuntimeSettings : AccountProvisioner
    {

        TelesEnterpriseManager _telesEnterpriseManager = new TelesEnterpriseManager();
        AccountBEManager _accountBEManager = new AccountBEManager();
        TelesSiteManager _telesSiteManager = new TelesSiteManager();
        TelesUserManager _telesUserManager = new TelesUserManager();

        public override void Execute(IAccountProvisioningContext context)
        {
            var definitionSettings = context.DefinitionSettings as ChangeUsersRGsDefinitionSettings;
            definitionSettings.ThrowIfNull("definitionSettings");
            var account = _accountBEManager.GetAccount(context.AccountBEDefinitionId, context.AccountId);
            account.ThrowIfNull("account", context.AccountId);
            context.WriteTrackingMessage(LogEntryType.Information, string.Format("Start loading users to {0}.", context.ActionDefinitionName));
            List<UsersToChangeRG> usersToChangeRG = GetUsersToChangeRG(context, account, definitionSettings);
            context.WriteTrackingMessage(LogEntryType.Information, string.Format("Start {0} users.", context.ActionDefinitionName));
            ChangeRGsAndUpdateState(context, usersToChangeRG, definitionSettings);
        }

        private List<UsersToChangeRG> GetUsersToChangeRG(IAccountProvisioningContext context, Account account, ChangeUsersRGsDefinitionSettings definitionSettings)
        {
            List<UsersToChangeRG> usersToChangeRG = new List<UsersToChangeRG>();
            if (account.TypeId == definitionSettings.CompanyTypeId)
            {
                AddUsersToChangeRGfromCompany(context, account, usersToChangeRG, definitionSettings);
            }

            else if (account.TypeId == definitionSettings.SiteTypeId)
            {
                EnterpriseAccountMappingInfo enterpriseAccountMappingInfo = null;
                if (account.ParentAccountId.HasValue)
                {
                    var parentCompanyAccount = _accountBEManager.GetSelfOrParentAccountOfType(context.AccountBEDefinitionId, account.ParentAccountId.Value, definitionSettings.CompanyTypeId);
                    if (parentCompanyAccount != null)
                        enterpriseAccountMappingInfo = _accountBEManager.GetExtendedSettings<EnterpriseAccountMappingInfo>(parentCompanyAccount);
                }
                string mappedTelesSiteId;
                AddUsersToChangeRGfromSite(context, account, enterpriseAccountMappingInfo, usersToChangeRG, definitionSettings, out mappedTelesSiteId);
            }
            else if (definitionSettings.UserTypeId.HasValue && definitionSettings.UserTypeId.Value == account.TypeId)
            {
                SiteAccountMappingInfo siteAccountMappingInfo = null;
                if (account.ParentAccountId.HasValue)
                {
                    var parentSiteAccount = _accountBEManager.GetSelfOrParentAccountOfType(context.AccountBEDefinitionId, account.ParentAccountId.Value, definitionSettings.SiteTypeId);
                    if (parentSiteAccount != null)
                        siteAccountMappingInfo = _accountBEManager.GetExtendedSettings<SiteAccountMappingInfo>(parentSiteAccount);
                }
                string mappedTelesUserId;
                AddUsersToChangeRGfromUser(context, account, siteAccountMappingInfo, usersToChangeRG, definitionSettings, out mappedTelesUserId);
            }
            return usersToChangeRG;
        }

        private void AddUsersToChangeRGfromCompany(IAccountProvisioningContext context, Account companyAccount, List<UsersToChangeRG> usersToChangeRG, ChangeUsersRGsDefinitionSettings definitionSettings)
        {
            EnterpriseAccountMappingInfo enterpriseAccountMappingInfo = _accountBEManager.GetExtendedSettings<EnterpriseAccountMappingInfo>(companyAccount);
            Dictionary<string, dynamic> telesSites = null;
            if (enterpriseAccountMappingInfo != null)
                telesSites = GetTelesSites(definitionSettings.VRConnectionId, enterpriseAccountMappingInfo.TelesEnterpriseId);
            List<Account> childAccounts = _accountBEManager.GetChildAccounts(context.AccountBEDefinitionId, companyAccount.AccountId, true);
            List<string> mappedTelesSiteIds = new List<string>();
            if (childAccounts != null)
            {
                foreach (var child in childAccounts)
                {
                    if (child.TypeId == definitionSettings.SiteTypeId)
                    {
                        string telesSiteId;
                        AddUsersToChangeRGfromSite(context, child, enterpriseAccountMappingInfo, usersToChangeRG, definitionSettings, out telesSiteId);
                        if (telesSiteId != null)
                            mappedTelesSiteIds.Add(telesSiteId);
                    }
                }
            }
            if (telesSites != null)
            {
                var telesUsersToChangeRG = new List<TelesUser>();
                foreach (var telesSiteEntry in telesSites)
                {
                    if (mappedTelesSiteIds.Contains(telesSiteEntry.Key))//teles site is mapped to Retail Account of type site and should be already executed in previous block of code
                        continue;
                    var siteTelesUsersToChangeRG = GetTelesUsersToChangeRGFromTelesSite(context, telesSiteEntry.Key, null, definitionSettings);
                    if (siteTelesUsersToChangeRG != null && siteTelesUsersToChangeRG.Count > 0)
                        telesUsersToChangeRG.AddRange(siteTelesUsersToChangeRG);
                }
               
                usersToChangeRG.Add(new UsersToChangeRG(companyAccount)
                {
                    TelesUsers = telesUsersToChangeRG
                });
                
            }
        }

        private void AddUsersToChangeRGfromSite(IAccountProvisioningContext context, Account siteAccount, EnterpriseAccountMappingInfo enterpriseAccountMappingInfo, List<UsersToChangeRG> usersToChangeRG, ChangeUsersRGsDefinitionSettings definitionSettings, out string telesSiteId)
        {
            SiteAccountMappingInfo siteAccountMappingInfo = _accountBEManager.GetExtendedSettings<SiteAccountMappingInfo>(siteAccount);

            List<string> mappedTelesUserIds = new List<string>();
            if (definitionSettings.UserTypeId.HasValue)
            {
                List<Account> childAccounts = _accountBEManager.GetChildAccounts(context.AccountBEDefinitionId, siteAccount.AccountId, true);
                if (childAccounts != null)
                {
                    foreach (var child in childAccounts)
                    {
                        if (child.TypeId == definitionSettings.UserTypeId.Value)
                        {
                            string mappedTelesUserId;
                            AddUsersToChangeRGfromUser(context, child, siteAccountMappingInfo, usersToChangeRG, definitionSettings, out mappedTelesUserId);
                            if (mappedTelesUserId != null)
                                mappedTelesUserIds.Add(mappedTelesUserId);
                        }
                    }
                }
            }

            if (siteAccountMappingInfo != null)
            {
                telesSiteId = siteAccountMappingInfo.TelesSiteId;
                if(telesSiteId != null)
                {
                    var siteTelesUsersToChangeRG = GetTelesUsersToChangeRGFromTelesSite(context, telesSiteId, mappedTelesUserIds, definitionSettings);

                    usersToChangeRG.Add(new UsersToChangeRG(siteAccount)
                    {
                        TelesUsers = siteTelesUsersToChangeRG
                    });
                }
            }
            else
            {
                telesSiteId = null;
            }
        }

        private void AddUsersToChangeRGfromUser(IAccountProvisioningContext context, Account userAccount, SiteAccountMappingInfo siteAccountMappingInfo, List<UsersToChangeRG> usersToChangeRG, ChangeUsersRGsDefinitionSettings definitionSettings, out string mappedTelesUserId)
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
                if(telesSiteId != null)
                {
                    List<string> existingRoutingGroups;
                    string newRoutingGroup;
                    bool? shouldUpdate;
                    GetTelesSiteRoutingGroups(context, telesSiteId, definitionSettings, out existingRoutingGroups, out newRoutingGroup, out shouldUpdate);
                    var currentTelesUser = GetTelesUser(definitionSettings.VRConnectionId, mappedTelesUserId);

                    TelesUser telesUser;
                    if (ShouldChangeTelesUserRG(context, mappedTelesUserId, currentTelesUser, telesSiteId, existingRoutingGroups, newRoutingGroup, definitionSettings, shouldUpdate, out telesUser))
                    {
                        usersToChangeRG.Add(new UsersToChangeRG(userAccount)
                        {
                            TelesUsers = new List<TelesUser>{
                    telesUser
                    },
                        });
                    }
                }
              
            }
          
        }

        private List<TelesUser> GetTelesUsersToChangeRGFromTelesSite(IAccountProvisioningContext context, string telesSiteId, List<string> excludedUserIds, ChangeUsersRGsDefinitionSettings definitionSettings)
        {
            List<TelesUser> telesUsersToChangeRG = new List<TelesUser>();
            Dictionary<string, dynamic> telesSiteUsers = GetTelesSiteUsers(definitionSettings.VRConnectionId, telesSiteId);
            if (telesSiteUsers != null && telesSiteUsers.Count > 0)
            {
                List<string> existingRoutingGroups;
                string newRoutingGroup;
                bool? shouldUpdate;
                GetTelesSiteRoutingGroups(context, telesSiteId, definitionSettings, out existingRoutingGroups, out newRoutingGroup, out shouldUpdate);
                foreach (var telesUserEntry in telesSiteUsers)
                {
                    if (excludedUserIds != null && excludedUserIds.Contains(telesUserEntry.Key))
                        continue;
                    TelesUser telesUser;
                    if (ShouldChangeTelesUserRG(context, telesUserEntry.Key, telesUserEntry.Value,telesSiteId, existingRoutingGroups, newRoutingGroup, definitionSettings,shouldUpdate, out telesUser))
                        telesUsersToChangeRG.Add(telesUser);
                }
            }
            return telesUsersToChangeRG;
        }

        private void GetTelesSiteRoutingGroups(IAccountProvisioningContext context, string telesSiteId, ChangeUsersRGsDefinitionSettings definitionSettings, out  List<string> existingRoutingGroups, out string newRoutingGroup, out bool? shouldUpdate)
        {
            shouldUpdate = null;
            Dictionary<string, dynamic> siteRoutingGroups = GetSiteRoutingGroups(definitionSettings.VRConnectionId, telesSiteId);
            existingRoutingGroups = null;
            newRoutingGroup = null;
            if (siteRoutingGroups != null)
            {
                foreach (dynamic siteRoutingGroup in siteRoutingGroups.Values)
                {
                    string siteRoutingGroupId = siteRoutingGroup.id.ToString();
                    if (definitionSettings.ExistingRoutingGroupCondition != null)
                    {
                        if (existingRoutingGroups == null)
                            existingRoutingGroups = new List<string>();
                        RoutingGroupConditionContext oldcontext = new RoutingGroupConditionContext
                        {
                            RoutingGroupName = siteRoutingGroup.name
                        };
                        if (definitionSettings.ExistingRoutingGroupCondition.Evaluate(oldcontext))
                        {
                            existingRoutingGroups.Add(siteRoutingGroupId);
                        }
                    }
                    RoutingGroupConditionContext newcontext = new RoutingGroupConditionContext
                    {
                        RoutingGroupName = siteRoutingGroup.name
                    };
                    if (definitionSettings.NewRoutingGroupCondition.Evaluate(newcontext))
                    {
                        if (newRoutingGroup != null)
                        {
                            switch (definitionSettings.NewRGMultiMatchHandling)
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
                switch (definitionSettings.NewRGNoMatchHandling)
                {
                    case NewRGNoMatchHandling.Skip:
                        shouldUpdate = false;
                        break;
                    case NewRGNoMatchHandling.Stop: throw new Exception("No routing group available for new routing group condition.");
                }
            }
            if (definitionSettings.ExistingRoutingGroupCondition == null)
            {
                shouldUpdate = true;
            }
            else if (existingRoutingGroups == null || existingRoutingGroups.Count == 0)
            {
                switch (definitionSettings.ExistingRGNoMatchHandling)
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

        private bool ShouldChangeTelesUserRG(IAccountProvisioningContext context, string telesUserId, dynamic telesUserObject, string telesSiteId, List<string> existingRoutingGroups, string newRoutingGroup, ChangeUsersRGsDefinitionSettings definitionSettings,bool? shouldUpdate, out TelesUser telesUser)
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
      
        private void ChangeRGsAndUpdateState(IAccountProvisioningContext context, List<UsersToChangeRG> usersToChangeRG, ChangeUsersRGsDefinitionSettings definitionSettings)
        {
            if (usersToChangeRG != null)
            {
                foreach (var userToChange in usersToChangeRG)
                {
                    if (userToChange.TelesUsers != null)
                    {
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
                        ChURGsActionCh chURGsActionCh = existingAccountState.ChangesByActionType.GetOrCreateItem(definitionSettings.ActionType);
                        if (chURGsActionCh.ChangesByUser == null)
                            chURGsActionCh.ChangesByUser = new Dictionary<string, ChURGsUserCh>();

                        List<string> usersNames = new List<string>();
                        foreach (var user in userToChange.TelesUsers)
                        {
                            UpdateUser(definitionSettings.VRConnectionId, user.User);
                            usersNames.Add(user.User.loginName.Value);
                            if (usersNames.Count == 10)
                            {
                                WriteUsersProccessedTrackingMessage(context, usersNames);
                                usersNames = new List<string>();
                            }
                            if (definitionSettings.SaveChangesToAccountState)
                            {
                                ChURGsUserCh chURGsUserCh = chURGsActionCh.ChangesByUser.GetOrCreateItem(user.UserId);
                                chURGsUserCh.ChangedRGId = user.NewRoutingGroupId;
                                chURGsUserCh.OriginalRGId = user.OldRoutingGroupId;
                                chURGsUserCh.SiteId = user.SiteId;
                                
                            }

                        }
                        if (definitionSettings.SaveChangesToAccountState)
                        {
                            if (_accountBEManager.UpdateAccountExtendedSetting<ChangeUsersRGsAccountState>(context.AccountBEDefinitionId, userToChange.Account.AccountId, existingAccountState))
                            {
                                context.TrackActionExecuted(userToChange.Account.AccountId, null, existingAccountState);
                            };
                        }
                        WriteUsersProccessedTrackingMessage(context, usersNames);
                        context.WriteTrackingMessage(LogEntryType.Information, string.Format("Finish processing account: {0}.", _accountBEManager.GetAccountName(userToChange.Account)));
                    }

                }
            }
        }

        #region Private Classes

        private class UsersToChangeRG
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

        private class TelesUser
        {
            public string UserId { get; set; }

            public dynamic User { get; set; }

            public string SiteId { get; set; }

            public string OldRoutingGroupId { get; set; }

            public string NewRoutingGroupId { get; set; }
        }

        #endregion

        Dictionary<string, dynamic> GetTelesSites(Guid vrConnectionId, string telesEnterpriseId)
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
        dynamic GetTelesUser(Guid vrConnectionId, string telesSiteId)
        {
            return _telesUserManager.GetUser(vrConnectionId, telesSiteId);
        }

        Dictionary<string, dynamic> GetTelesSiteUsers(Guid vrConnectionId, string siteId)
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
        Dictionary<string, dynamic> GetSiteRoutingGroups(Guid vrConnectionId, string siteId)
        {
            return _telesSiteManager.GetSiteRoutingGroups(vrConnectionId, siteId);
        }
        void UpdateUser(Guid vrConnectionId, dynamic user)
        {
            _telesUserManager.UpdateUser(vrConnectionId, user);
        }

        private void WriteUsersProccessedTrackingMessage(IAccountProvisioningContext context, List<string> usersNames)
        {
            if (usersNames.Count > 0)
            {
                context.WriteTrackingMessage(LogEntryType.Information, string.Format("Users processed: {0}. ", String.Join<string>(",", usersNames)));
            }
        }

    }
}

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
        TelesEnterpriseManager telesEnterpriseManager = new TelesEnterpriseManager();
        AccountBEManager accountBEManager = new AccountBEManager();
        TelesSiteManager telesSiteManager = new TelesSiteManager();
        public override void Execute(IAccountProvisioningContext context)
        {
            var definitionSettings = context.DefinitionSettings as ChangeUsersRGsDefinitionSettings;
            var account = accountBEManager.GetAccount(context.AccountBEDefinitionId, context.AccountId);
            if (!TelesAccountCondition.AllowChangeUserRGs(account, definitionSettings.CompanyTypeId, definitionSettings.SiteTypeId, definitionSettings.ActionType))
            {
                throw new Exception("Not Allow to Change User Routing Groups");
            }
            account.ThrowIfNull("account", context.AccountId);
            if(account.TypeId == definitionSettings.CompanyTypeId) // Process if company
            {
                var enterpriseAccountMappingInfo = new AccountBEManager().GetExtendedSettings<EnterpriseAccountMappingInfo>(account);
                if (enterpriseAccountMappingInfo != null)
                {
                   
                    context.WriteTrackingMessage(LogEntryType.Information, string.Format("Loading all sites for enterpriseId {0}.", enterpriseAccountMappingInfo.TelesEnterpriseId));
                    var mappedSites = telesSiteManager.GetCachedAccountsBySites(context.AccountBEDefinitionId, enterpriseAccountMappingInfo.TelesEnterpriseId);
                    var sites = GetSites(definitionSettings.VRConnectionId, enterpriseAccountMappingInfo.TelesEnterpriseId);
                    context.WriteTrackingMessage(LogEntryType.Information, string.Format("All sites for enterpriseId {0} loaded.", enterpriseAccountMappingInfo.TelesEnterpriseId));
                    if (sites != null)
                    {
                        Dictionary<string, ChURGsUserCh> changesByUser = new Dictionary<string, ChURGsUserCh>();

                        List<dynamic> usersToBeChangedByCompany;
                        ChangeUsersRGForeachSite(context, definitionSettings, sites, mappedSites, changesByUser, out usersToBeChangedByCompany);
                        UpdateChangedUsers(definitionSettings, usersToBeChangedByCompany);
                        if (changesByUser != null)
                            UpdateChangedUsersState(context, definitionSettings, context.AccountId, changesByUser);
                    }
                }
            }
            else if (account.TypeId == definitionSettings.SiteTypeId) // Process if site
            {
                var siteAccountMappingInfo = new AccountBEManager().GetExtendedSettings<SiteAccountMappingInfo>(context.AccountBEDefinitionId, context.AccountId);
                if (siteAccountMappingInfo != null)
                {
                    var siteName = telesSiteManager.GetSiteName(definitionSettings.VRConnectionId, siteAccountMappingInfo.TelesSiteId);
                    context.WriteTrackingMessage(LogEntryType.Information, string.Format("Begin processing site {0}.", siteName));
                    Dictionary<string, dynamic> siteRoutingGroups = GetSiteRoutingGroups(definitionSettings.VRConnectionId, siteAccountMappingInfo.TelesSiteId);
                    ChangeUsersRoutingGroupForMappedSite(context, definitionSettings, siteRoutingGroups, siteAccountMappingInfo.TelesSiteId, context.AccountId);
                    context.WriteTrackingMessage(LogEntryType.Information, string.Format("End processing site {0}.", siteName));
                }
            }
        }
        void ChangeUsersRGForeachSite(IAccountProvisioningContext context, ChangeUsersRGsDefinitionSettings definitionSettings, IEnumerable<dynamic> sites, Dictionary<string, long> mappedSites, Dictionary<string, ChURGsUserCh> changesByUser, out List<dynamic> usersToBeChangedByCompany)
        {
            usersToBeChangedByCompany = new List<dynamic>();
            foreach (var site in sites)
            {
                string siteId = site.id.ToString();
                Dictionary<string, dynamic> siteRoutingGroups = GetSiteRoutingGroups(definitionSettings.VRConnectionId, siteId);
                context.WriteTrackingMessage(LogEntryType.Information, string.Format("Routing groups for site {0} loaded.", site.name));
                if (siteRoutingGroups != null)
                {
                    long accountId = -1;

                    if (mappedSites != null && mappedSites.TryGetValue(siteId, out accountId))
                    {
                        ChangeUsersRoutingGroupForMappedSite(context, definitionSettings, siteRoutingGroups, siteId, accountId);
                    }else
                    {
                        ChangeUsersRoutingGroup(context, definitionSettings, siteRoutingGroups, siteId, changesByUser, usersToBeChangedByCompany);
                    }

                }
                context.WriteTrackingMessage(LogEntryType.Information, string.Format("Site {0} processed successfully.", site.name));
            }
        }
        void ChangeUsersRoutingGroupForMappedSite(IAccountProvisioningContext context, ChangeUsersRGsDefinitionSettings definitionSettings, Dictionary<string, dynamic> siteRoutingGroups, string siteId, long accountId)
        {
            List<dynamic> usersToBeChangedByCompany = new List<dynamic>();
        
            Dictionary<string, ChURGsUserCh> changesByUser = new Dictionary<string, ChURGsUserCh>();
            ChangeUsersRoutingGroup(context, definitionSettings, siteRoutingGroups, siteId, changesByUser, usersToBeChangedByCompany);
            UpdateChangedUsers(definitionSettings, usersToBeChangedByCompany);
            if (changesByUser != null)
                UpdateChangedUsersState(context, definitionSettings, accountId, changesByUser);
        }
        void ChangeUsersRoutingGroup(IAccountProvisioningContext context, ChangeUsersRGsDefinitionSettings definitionSettings, Dictionary<string, dynamic> siteRoutingGroups, string siteId, Dictionary<string, ChURGsUserCh> changesByUser, List<dynamic> usersToBeChangedByCompany)
        {
            List<string> existingRoutingGroups = null;
            string newRoutingGroup = null;
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
                            case NewRGMultiMatchHandling.Skip: return;
                            case NewRGMultiMatchHandling.Stop: throw new Exception("More than one routing group available for new routing group condition.");
                        }
                    }
                    newRoutingGroup = siteRoutingGroupId;
                }
            }
            if (newRoutingGroup == null)
            {
                switch(definitionSettings.NewRGNoMatchHandling)
                {
                    case NewRGNoMatchHandling.Skip: return;
                    case NewRGNoMatchHandling.Stop: throw new Exception("No routing group available for new routing group condition.");
                }
            }
            if (definitionSettings.ExistingRoutingGroupCondition == null)
            {
                ChangeUsersRoutingGroup(context, definitionSettings, siteId, existingRoutingGroups, newRoutingGroup, changesByUser, true, usersToBeChangedByCompany);
            }else if (existingRoutingGroups == null)
            {
                switch (definitionSettings.ExistingRGNoMatchHandling)
                {
                    case ExistingRGNoMatchHandling.Skip: return;
                    case ExistingRGNoMatchHandling.UpdateAll:
                        ChangeUsersRoutingGroup(context, definitionSettings, siteId, existingRoutingGroups, newRoutingGroup, changesByUser, true, usersToBeChangedByCompany);
                        break;
                    case ExistingRGNoMatchHandling.Stop:
                        if (existingRoutingGroups == null)
                            throw new Exception("No routing group available for existing routing group condition.");
                        break;
                }
            }
            else
            {
                ChangeUsersRoutingGroup(context, definitionSettings, siteId, existingRoutingGroups, newRoutingGroup, changesByUser, false, usersToBeChangedByCompany);
            }
        }
        void ChangeUsersRoutingGroup(IAccountProvisioningContext context, ChangeUsersRGsDefinitionSettings definitionSettings, string siteId, List<string> existingRoutingGroups, string newRoutingGroup, Dictionary<string, ChURGsUserCh> changesByUser, bool updateAll, List<dynamic> usersToBeChangedByCompany)
        {
            var users = GetUsers(definitionSettings.VRConnectionId, siteId);
            if(users != null)
            {
                context.WriteTrackingMessage(LogEntryType.Information, string.Format("Start processing {0} users.", users.Count()));
                List<string> usersNames = new List<string>();
                foreach (var user in users)
                {
                    if (user.routingGroupId != newRoutingGroup && (updateAll || existingRoutingGroups.Contains(user.routingGroupId.ToString())))
                    {
                        usersNames.Add(user.loginName.Value);
                        if (definitionSettings.SaveChangesToAccountState)
                        {
                            if (changesByUser == null)
                            {
                                changesByUser = new Dictionary<string, ChURGsUserCh>();
                            }
                          
                            ChURGsUserCh chURGsUserCh;
                            string userId = user.id.ToString();
                            if (!changesByUser.TryGetValue(userId, out chURGsUserCh))
                            {
                                chURGsUserCh = new ChURGsUserCh { OriginalRGId = user.routingGroupId.ToString(), ChangedRGId = newRoutingGroup, SiteId = siteId };
                                changesByUser.Add(userId, chURGsUserCh);
                            }
                        }
                        user.routingGroupId = newRoutingGroup;
                        usersToBeChangedByCompany.Add(user);
                    }
                    if (usersNames.Count == 10)
                    {
                        WriteUsersProccessedTrackingMessage(context, usersNames);
                        usersNames = new List<string>();
                    }
                }
                WriteUsersProccessedTrackingMessage(context, usersNames);
                context.WriteTrackingMessage(LogEntryType.Information, string.Format("End processing {0} users.", users.Count()));
            }
            
        }
        void UpdateChangedUsers(ChangeUsersRGsDefinitionSettings definitionSettings,List<dynamic> changedUsersRoutingRG)
        {
            if (changedUsersRoutingRG != null)
            {
                foreach (var user in changedUsersRoutingRG)
                {
                    UpdateUser(definitionSettings.VRConnectionId, user);
                }
            }
        }
        void UpdateChangedUsersState(IAccountProvisioningContext context,ChangeUsersRGsDefinitionSettings definitionSettings, long accountId,Dictionary<string, ChURGsUserCh> changesByUser)
        {
            ChangeUsersRGsAccountState changeUsersRGsAccountState = new AccountBEManager().GetExtendedSettings<ChangeUsersRGsAccountState>(context.AccountBEDefinitionId, accountId);
            if (changeUsersRGsAccountState == null)
                changeUsersRGsAccountState = new ChangeUsersRGsAccountState();
            if (changeUsersRGsAccountState.ChangesByActionType == null)
                changeUsersRGsAccountState.ChangesByActionType = new Dictionary<string, ChURGsActionCh>();
            ChURGsActionCh chUSiteRGsActionCh;
            if (!changeUsersRGsAccountState.ChangesByActionType.TryGetValue(definitionSettings.ActionType, out chUSiteRGsActionCh))
            {
                chUSiteRGsActionCh = new ChURGsActionCh();
                changeUsersRGsAccountState.ChangesByActionType.Add(definitionSettings.ActionType, chUSiteRGsActionCh);
            }
            if (chUSiteRGsActionCh.ChangesByUser == null)
                chUSiteRGsActionCh.ChangesByUser = new Dictionary<string, ChURGsUserCh>();

            foreach (var user in changesByUser)
            {
                if (!chUSiteRGsActionCh.ChangesByUser.ContainsKey(user.Key))
                  chUSiteRGsActionCh.ChangesByUser.Add(user.Key, user.Value);
            }

            if (changeUsersRGsAccountState != null)
            {
                if (accountBEManager.UpdateAccountExtendedSetting<ChangeUsersRGsAccountState>(context.AccountBEDefinitionId, accountId, changeUsersRGsAccountState))
                {
                    context.TrackActionExecuted(null, changesByUser);
                };
            }

        }
        IEnumerable<dynamic> GetSites(Guid vrConnectionId, string telesEnterpriseId)
        {
            return telesSiteManager.GetSites(vrConnectionId, telesEnterpriseId);
        }
        IEnumerable<dynamic> GetUsers(Guid vrConnectionId, string siteId)
        {
            return telesEnterpriseManager.GetUsers(vrConnectionId, siteId);
        }
        Dictionary<string, dynamic> GetSiteRoutingGroups(Guid vrConnectionId, string siteId)
        {
            return telesSiteManager.GetSiteRoutingGroups(vrConnectionId, siteId);
        }
        void UpdateUser(Guid vrConnectionId, dynamic user)
        {
            telesEnterpriseManager.UpdateUser(vrConnectionId, user);
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

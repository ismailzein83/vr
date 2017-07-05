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
            account.ThrowIfNull("account", context.AccountId);
            if(account.TypeId == definitionSettings.CompanyTypeId) // Process if company
            {
                var enterpriseAccountMappingInfo = new AccountBEManager().GetExtendedSettings<EnterpriseAccountMappingInfo>(account);
                if (enterpriseAccountMappingInfo != null)
                {
                    ChangeUsersRGsAccountState changeUsersRGsAccountState = new AccountBEManager().GetExtendedSettings<ChangeUsersRGsAccountState>(account);
                    if (changeUsersRGsAccountState == null)
                        changeUsersRGsAccountState = new ChangeUsersRGsAccountState();
                    if (changeUsersRGsAccountState.ChangesByActionType == null)
                        changeUsersRGsAccountState.ChangesByActionType = new Dictionary<string, ChURGsActionCh>();

                    ChURGsActionCh chUCompanyRGsActionCh;
                    ChURGsActionCh newChUCompanyRGsActionCh = new ChURGsActionCh();
                    if (!changeUsersRGsAccountState.ChangesByActionType.TryGetValue(definitionSettings.ActionType, out chUCompanyRGsActionCh))
                    {
                        chUCompanyRGsActionCh = new ChURGsActionCh();
                        changeUsersRGsAccountState.ChangesByActionType.Add(definitionSettings.ActionType, chUCompanyRGsActionCh);
                        chUCompanyRGsActionCh.Status = ChURGsActionChStatus.Blocked;
                        newChUCompanyRGsActionCh.Status = ChURGsActionChStatus.Blocked;
                    }
                    context.WriteTrackingMessage(LogEntryType.Information, string.Format("Loading all sites for enterpriseId {0}.", enterpriseAccountMappingInfo.TelesEnterpriseId));
                    var mappedSites = telesSiteManager.GetCachedAccountsBySites(context.AccountBEDefinitionId, enterpriseAccountMappingInfo.TelesEnterpriseId);
                    var sites = GetSites(definitionSettings.VRConnectionId, enterpriseAccountMappingInfo.TelesEnterpriseId);
                    context.WriteTrackingMessage(LogEntryType.Information, string.Format("All sites for enterpriseId {0} loaded.", enterpriseAccountMappingInfo.TelesEnterpriseId));
                    if (sites != null)
                    {
                        List<dynamic> usersToBlock = BlockUsersForeachSite(context, definitionSettings, sites, mappedSites, chUCompanyRGsActionCh, newChUCompanyRGsActionCh);
                        UpdateBlockedUsers(definitionSettings, usersToBlock);
                        if (chUCompanyRGsActionCh != null)
                            UpdateBlockedUsersState(context, context.AccountId, changeUsersRGsAccountState, newChUCompanyRGsActionCh);
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
                    BlockMappedSiteUsers(context, definitionSettings, siteRoutingGroups, siteAccountMappingInfo.TelesSiteId, context.AccountId);
                    context.WriteTrackingMessage(LogEntryType.Information, string.Format("End processing site {0}.", siteName));
                }
            }
        }
        List<dynamic> BlockUsersForeachSite(IAccountProvisioningContext context, ChangeUsersRGsDefinitionSettings definitionSettings, IEnumerable<dynamic> sites, Dictionary<string, long> mappedSites, ChURGsActionCh chUCompanyRGsActionCh, ChURGsActionCh newChUCompanyRGsActionCh)
        {
            List<dynamic> usersToBlock = new List<dynamic>();
            foreach (var site in sites)
            {
                string siteId = site.id.ToString();
                context.WriteTrackingMessage(LogEntryType.Information, string.Format("Begin processing site {0}.", site.name));
                context.WriteTrackingMessage(LogEntryType.Information, string.Format("Loading routing groups for site {0}.", site.name));
                Dictionary<string, dynamic> siteRoutingGroups = GetSiteRoutingGroups(definitionSettings.VRConnectionId, siteId);
                context.WriteTrackingMessage(LogEntryType.Information, string.Format("Routing groups for site {0} loaded.", site.name));
                if (siteRoutingGroups != null)
                {
                    long accountId = -1;
                    
                    if (mappedSites.TryGetValue(siteId, out accountId))
                    {
                        BlockMappedSiteUsers(context, definitionSettings, siteRoutingGroups, siteId, accountId);
                    }else
                    {
                        GetUsersToBlock(context, definitionSettings, usersToBlock, siteRoutingGroups, siteId, chUCompanyRGsActionCh, newChUCompanyRGsActionCh);
                    }

                }
                context.WriteTrackingMessage(LogEntryType.Information, string.Format("End processing site {0}.", site.name));
            }
            return usersToBlock;
        }
        void BlockMappedSiteUsers(IAccountProvisioningContext context, ChangeUsersRGsDefinitionSettings definitionSettings, Dictionary<string, dynamic> siteRoutingGroups, string siteId, long accountId)
        {
            List<dynamic> siteUsersToBlock = new List<dynamic>();

            ChangeUsersRGsAccountState changeSiteUsersRGsState = new AccountBEManager().GetExtendedSettings<ChangeUsersRGsAccountState>(context.AccountBEDefinitionId, accountId);
            if (changeSiteUsersRGsState == null)
                changeSiteUsersRGsState = new ChangeUsersRGsAccountState();
            if (changeSiteUsersRGsState.ChangesByActionType == null)
                changeSiteUsersRGsState.ChangesByActionType = new Dictionary<string, ChURGsActionCh>();

            ChURGsActionCh chUSiteRGsActionCh;
            ChURGsActionCh newChUSiteRGsActionCh = new ChURGsActionCh();
            if (!changeSiteUsersRGsState.ChangesByActionType.TryGetValue(definitionSettings.ActionType, out chUSiteRGsActionCh))
            {
                chUSiteRGsActionCh = new ChURGsActionCh();
                changeSiteUsersRGsState.ChangesByActionType.Add(definitionSettings.ActionType, chUSiteRGsActionCh);
            }

            GetUsersToBlock(context, definitionSettings, siteUsersToBlock, siteRoutingGroups, siteId, chUSiteRGsActionCh, newChUSiteRGsActionCh);
            UpdateBlockedUsers(definitionSettings, siteUsersToBlock);
            if (chUSiteRGsActionCh != null && chUSiteRGsActionCh.ChangesByUser != null && chUSiteRGsActionCh.ChangesByUser.Count != 0)
                UpdateBlockedUsersState(context, accountId, changeSiteUsersRGsState, newChUSiteRGsActionCh);
        }
        void GetUsersToBlock(IAccountProvisioningContext context, ChangeUsersRGsDefinitionSettings definitionSettings, List<dynamic> usersToBlock, Dictionary<string, dynamic> siteRoutingGroups, string siteId, ChURGsActionCh chURGsActionCh, ChURGsActionCh newChURGsActionCh)
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
                ProcessUsersToBlock(context,definitionSettings, siteId, existingRoutingGroups, newRoutingGroup, usersToBlock, chURGsActionCh,newChURGsActionCh, true);
            }else if (existingRoutingGroups == null)
            {
                switch (definitionSettings.ExistingRGNoMatchHandling)
                {
                    case ExistingRGNoMatchHandling.Skip: return;
                    case ExistingRGNoMatchHandling.UpdateAll:
                        ProcessUsersToBlock(context,definitionSettings, siteId, existingRoutingGroups, newRoutingGroup, usersToBlock, chURGsActionCh,newChURGsActionCh, true);
                        break;
                    case ExistingRGNoMatchHandling.Stop:
                        if (existingRoutingGroups == null)
                            throw new Exception("No routing group available for existing routing group condition.");
                        break;
                }
            }
            else
            {
                ProcessUsersToBlock(context,definitionSettings, siteId, existingRoutingGroups, newRoutingGroup, usersToBlock, chURGsActionCh,newChURGsActionCh, false);
            }
        }
        void ProcessUsersToBlock(IAccountProvisioningContext context, ChangeUsersRGsDefinitionSettings definitionSettings, string siteId, List<string> existingRoutingGroups, string newRoutingGroup, List<dynamic> usersToBlock, ChURGsActionCh chURGsActionCh, ChURGsActionCh newChURGsActionCh, bool updateAll)
        {
            var users = GetUsers(definitionSettings.VRConnectionId, siteId);
            if(users != null)
            {
                context.WriteTrackingMessage(LogEntryType.Information, string.Format("Begin processing {0} users.", users.Count()));
                List<string> usersNames = new List<string>();
                foreach (var user in users)
                {
                    if (user.routingGroupId != newRoutingGroup && (updateAll || existingRoutingGroups.Contains(user.routingGroupId.ToString())))
                    {
                        if (usersNames.Count <= 10 )
                        {
                           usersNames.Add(user.loginName.Value);
                        }
                         
                        if (definitionSettings.SaveChangesToAccountState)
                        {
                            if (chURGsActionCh.ChangesByUser == null)
                            {
                                chURGsActionCh.ChangesByUser = new Dictionary<string, ChURGsUserCh>();
                            }
                            if(newChURGsActionCh.ChangesByUser == null)
                            {
                                newChURGsActionCh.ChangesByUser = new Dictionary<string, ChURGsUserCh>();
                            }
                            ChURGsUserCh chURGsUserCh;
                            string userId = user.id.ToString();
                            if (!chURGsActionCh.ChangesByUser.TryGetValue(userId, out chURGsUserCh))
                            {
                                chURGsUserCh = new ChURGsUserCh { OriginalRGId = user.routingGroupId.ToString(), ChangedRGId = newRoutingGroup, SiteId = siteId };
                                chURGsActionCh.ChangesByUser.Add(userId, chURGsUserCh);
                                newChURGsActionCh.ChangesByUser.Add(userId, chURGsUserCh);
                            }
                        }
                        user.routingGroupId = newRoutingGroup;
                        usersToBlock.Add(user);
                    }
                }
                if (usersNames.Count > 0)
                {
                    newChURGsActionCh.Status = ChURGsActionChStatus.Blocked;
                    chURGsActionCh.Status = ChURGsActionChStatus.Blocked;
                    context.WriteTrackingMessage(LogEntryType.Information, string.Format("Users processed: {0}. ", String.Join<string>(",", usersNames)));
                }
                context.WriteTrackingMessage(LogEntryType.Information, string.Format("End processing {0} users.", users.Count()));
            }
            
        }
        void UpdateBlockedUsers(ChangeUsersRGsDefinitionSettings definitionSettings,List<dynamic> usersToBlock)
        {
            if ( usersToBlock != null)
            {
                foreach (var userToBlock in usersToBlock)
                {
                    UpdateUser(definitionSettings.VRConnectionId, userToBlock);
                }
            }
        }
        void UpdateBlockedUsersState(IAccountProvisioningContext context,long accountId, ChangeUsersRGsAccountState changeUsersRGsAccountState,ChURGsActionCh newChURGsActionCh)
        {
            if (changeUsersRGsAccountState != null)
            {
                if (accountBEManager.UpdateAccountExtendedSetting<ChangeUsersRGsAccountState>(context.AccountBEDefinitionId, accountId, changeUsersRGsAccountState))
                {
                    context.TrackActionExecuted(null, newChURGsActionCh);
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
    }
}

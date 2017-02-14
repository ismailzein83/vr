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

namespace Retail.Teles.Business
{
    public class ChangeUsersRGsRuntimeSettings : AccountProvisioner
    {
        TelesEnterpriseManager telesEnterpriseManager = new TelesEnterpriseManager();
        AccountBEManager accountBEManager = new AccountBEManager();
        public override void Execute(IAccountProvisioningContext context)
        {
            var definitionSettings = context.DefinitionSettings as ChangeUsersRGsDefinitionSettings;

            var enterpriseAccountMappingInfo = new AccountBEManager().GetExtendedSettings<EnterpriseAccountMappingInfo>(context.AccountBEDefinitionId, context.AccountId);

            if (enterpriseAccountMappingInfo != null)
            {
                var changeUsersRGsAccountState = new AccountBEManager().GetExtendedSettings<ChangeUsersRGsAccountState>(context.AccountBEDefinitionId, context.AccountId);
                if (changeUsersRGsAccountState == null)
                    changeUsersRGsAccountState = new ChangeUsersRGsAccountState();
                if (changeUsersRGsAccountState.ChangesByActionType == null)
                    changeUsersRGsAccountState.ChangesByActionType = new Dictionary<string, ChURGsActionCh>();
               
                ChURGsActionCh chURGsActionCh;
                if (!changeUsersRGsAccountState.ChangesByActionType.TryGetValue(definitionSettings.ActionType, out chURGsActionCh))
                {
                    chURGsActionCh = new ChURGsActionCh();
                    changeUsersRGsAccountState.ChangesByActionType.Add(definitionSettings.ActionType, chURGsActionCh);
                }

                context.WriteTrackingMessage(LogEntryType.Information, string.Format("Loading all sites for enterpriseId {0}.", enterpriseAccountMappingInfo.TelesEnterpriseId));
                var sites = GetSites(definitionSettings.VRConnectionId, enterpriseAccountMappingInfo.TelesEnterpriseId);
                context.WriteTrackingMessage(LogEntryType.Information, string.Format("All sites for enterpriseId {0} loaded.", enterpriseAccountMappingInfo.TelesEnterpriseId));
                if (sites != null)
                {
                    var usersToBlock = GetUsersToBlockForeachSite(context,definitionSettings, sites, chURGsActionCh);
                    UpdateBlockedUsers(definitionSettings, usersToBlock);
                    
                    if (chURGsActionCh != null && chURGsActionCh.ChangesByUser != null && chURGsActionCh.ChangesByUser.Count != 0)
                        UpdateBlockedUsersState(context.AccountBEDefinitionId, context.AccountId, changeUsersRGsAccountState);
                }
            }
        }
        List<dynamic> GetUsersToBlockForeachSite(IAccountProvisioningContext context,ChangeUsersRGsDefinitionSettings definitionSettings, IEnumerable<dynamic> sites, ChURGsActionCh chURGsActionCh)
        {
            List<dynamic> usersToBlock = new List<dynamic>();
            foreach (var site in sites)
            {
                context.WriteTrackingMessage(LogEntryType.Information, string.Format("Begin processing site {0}.", site.name));
                context.WriteTrackingMessage(LogEntryType.Information, string.Format("Loading routing groups for site {0}.", site.name));
                Dictionary<dynamic, dynamic> siteRoutingGroups = GetSiteRoutingGroups(definitionSettings.VRConnectionId, site.id);
                context.WriteTrackingMessage(LogEntryType.Information, string.Format("Routing groups for site {0} loaded.", site.name));
                if (siteRoutingGroups != null)
                {
                    GetUsersToBlock(context,definitionSettings, usersToBlock, siteRoutingGroups, site.id, chURGsActionCh);
                }
                context.WriteTrackingMessage(LogEntryType.Information, string.Format("End processing site {0}.", site.name));
            }
            return usersToBlock;
        }
        void GetUsersToBlock(IAccountProvisioningContext context, ChangeUsersRGsDefinitionSettings definitionSettings, List<dynamic> usersToBlock, Dictionary<dynamic, dynamic> siteRoutingGroups, dynamic siteId, ChURGsActionCh chURGsActionCh)
        {

            List<dynamic> existingRoutingGroups = null;
            dynamic newRoutingGroup = null;
            foreach (dynamic siteRoutingGroup in siteRoutingGroups.Values)
            {
                if (definitionSettings.ExistingRoutingGroupCondition != null)
                {
                    if (existingRoutingGroups == null)
                       existingRoutingGroups = new List<dynamic>();
                    RoutingGroupConditionContext oldcontext = new RoutingGroupConditionContext
                    {
                        RoutingGroupName = siteRoutingGroup.name
                    };
                    if (definitionSettings.ExistingRoutingGroupCondition.Evaluate(oldcontext))
                    {
                        existingRoutingGroups.Add(siteRoutingGroup.id);
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
                    newRoutingGroup = siteRoutingGroup.id;
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
                ProcessUsersToBlock(context,definitionSettings, siteId, existingRoutingGroups, newRoutingGroup, usersToBlock, chURGsActionCh, true);
            }else if (existingRoutingGroups == null)
            {
                switch (definitionSettings.ExistingRGNoMatchHandling)
                {
                    case ExistingRGNoMatchHandling.Skip: return;
                    case ExistingRGNoMatchHandling.UpdateAll:
                        ProcessUsersToBlock(context,definitionSettings, siteId, existingRoutingGroups, newRoutingGroup, usersToBlock, chURGsActionCh, true);
                        break;
                    case ExistingRGNoMatchHandling.Stop:
                        if (existingRoutingGroups == null)
                            throw new Exception("No routing group available for existing routing group condition.");
                        break;
                }
            }
            else
            {
                ProcessUsersToBlock(context,definitionSettings, siteId, existingRoutingGroups, newRoutingGroup, usersToBlock, chURGsActionCh, false);
            }
        }
        void ProcessUsersToBlock(IAccountProvisioningContext context, ChangeUsersRGsDefinitionSettings definitionSettings, dynamic siteId, List<dynamic> existingRoutingGroups, dynamic newRoutingGroup, List<dynamic> usersToBlock, ChURGsActionCh chURGsActionCh, bool updateAll)
        {
            var users = GetUsers(definitionSettings.VRConnectionId, siteId);
            if(users != null)
            {
                context.WriteTrackingMessage(LogEntryType.Information, string.Format("Begin processing {0} users.", users.Count));
                List<string> usersNames = new List<string>(); ;
                foreach (var user in users)
                {
                    if (user.routingGroupId != newRoutingGroup && (updateAll || existingRoutingGroups.Contains(user.routingGroupId)))
                    {
                        if (usersNames.Count <= 10 )
                        {
                           usersNames.Add(user.loginName.Value);
                        }
                         
                        if (definitionSettings.SaveChangesToAccountState)
                        {
                            if (chURGsActionCh.ChangesByUser == null)
                                chURGsActionCh.ChangesByUser = new Dictionary<dynamic, ChURGsUserCh>();
                            ChURGsUserCh chURGsUserCh;
                            if (!chURGsActionCh.ChangesByUser.TryGetValue(user.id, out chURGsUserCh))
                            {
                                chURGsActionCh.ChangesByUser.Add(user.id, new ChURGsUserCh { OriginalRGId = user.routingGroupId });
                            }
                        }
                        user.routingGroupId = newRoutingGroup;
                        usersToBlock.Add(user);
                    }
                }
                if (usersNames.Count > 0)
                {
                    context.WriteTrackingMessage(LogEntryType.Information, string.Format("Users processed: {0}. ", String.Join<string>(",", usersNames)));
                }
                context.WriteTrackingMessage(LogEntryType.Information, string.Format("End processing {0} users.", users.Count));
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
        void UpdateBlockedUsersState(Guid accountBEDefinition, long accountId, ChangeUsersRGsAccountState changeUsersRGsAccountState)
        {
            if (changeUsersRGsAccountState != null)
            {
                accountBEManager.UpdateAccountExtendedSetting<ChangeUsersRGsAccountState>(accountBEDefinition, accountId, changeUsersRGsAccountState);
            }

        }
        IEnumerable<dynamic> GetSites(Guid vrConnectionId, dynamic telesEnterpriseId)
        {
            return telesEnterpriseManager.GetSites(vrConnectionId, telesEnterpriseId);
        }
        IEnumerable<dynamic> GetUsers(Guid vrConnectionId, dynamic siteId)
        {
            return telesEnterpriseManager.GetUsers(vrConnectionId, siteId);
        }
        Dictionary<dynamic, dynamic> GetSiteRoutingGroups(Guid vrConnectionId, dynamic siteId)
        {
            return  telesEnterpriseManager.GetSiteRoutingGroups(vrConnectionId, siteId);
        }
        void UpdateUser(Guid vrConnectionId, dynamic user)
        {
            telesEnterpriseManager.UpdateUser(vrConnectionId, user);
        }
    }
}

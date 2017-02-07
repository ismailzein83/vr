using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Teles.Business.Provisioning;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
               

                var sites = GetSites(definitionSettings.SwitchId, enterpriseAccountMappingInfo.TelesEnterpriseId);
                if (sites != null)
                {
                    var usersToBlock = GetUsersToBlockForeachSite(definitionSettings, sites, chURGsActionCh);
                    UpdateBlockedUsers(definitionSettings, usersToBlock);
                    
                    if (chURGsActionCh != null && chURGsActionCh.ChangesByUser != null && chURGsActionCh.ChangesByUser.Count != 0)
                        UpdateBlockedUsersState(context.AccountBEDefinitionId, context.AccountId, changeUsersRGsAccountState);
                }
            }
        }
        List<dynamic> GetUsersToBlockForeachSite(ChangeUsersRGsDefinitionSettings definitionSettings, IEnumerable<dynamic> sites, ChURGsActionCh chURGsActionCh)
        {
            List<dynamic> usersToBlock = new List<dynamic>();
            foreach (var site in sites)
            {
                Dictionary<dynamic, dynamic> siteRoutingGroups = GetSiteRoutingGroups(definitionSettings.SwitchId, site.id);
                if (siteRoutingGroups != null)
                {
                    GetUsersToBlock(definitionSettings, usersToBlock, siteRoutingGroups, site.id, chURGsActionCh);
                }
            }
            return usersToBlock;
        }
        void GetUsersToBlock(ChangeUsersRGsDefinitionSettings definitionSettings, List<dynamic> usersToBlock, Dictionary<dynamic, dynamic> siteRoutingGroups, dynamic siteId, ChURGsActionCh chURGsActionCh)
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
                ProcessUsersToBlock(definitionSettings, siteId, existingRoutingGroups, newRoutingGroup, usersToBlock, chURGsActionCh, true);
            }else if (existingRoutingGroups == null)
            {
                switch (definitionSettings.ExistingRGNoMatchHandling)
                {
                    case ExistingRGNoMatchHandling.Skip: return;
                    case ExistingRGNoMatchHandling.UpdateAll:
                        ProcessUsersToBlock(definitionSettings, siteId, existingRoutingGroups, newRoutingGroup, usersToBlock, chURGsActionCh, true);
                        break;
                    case ExistingRGNoMatchHandling.Stop:
                        if (existingRoutingGroups == null)
                            throw new Exception("No routing group available for existing routing group condition.");
                        break;
                }
            }
            else
            {
                ProcessUsersToBlock(definitionSettings, siteId, existingRoutingGroups, newRoutingGroup, usersToBlock, chURGsActionCh, false);
            }
        }
        void ProcessUsersToBlock(ChangeUsersRGsDefinitionSettings definitionSettings, dynamic siteId, List<dynamic> existingRoutingGroups, dynamic newRoutingGroup, List<dynamic> usersToBlock, ChURGsActionCh chURGsActionCh, bool updateAll)
        {
            var users = GetUsers(definitionSettings.SwitchId, siteId);
            if(users != null)
            {
                foreach (var user in users)
                {
                    if (user.routingGroupId != newRoutingGroup && (updateAll || existingRoutingGroups.Contains(user.routingGroupId)))
                    {
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
            }
            
        }
        void UpdateBlockedUsers(ChangeUsersRGsDefinitionSettings definitionSettings,List<dynamic> usersToBlock)
        {
            if ( usersToBlock != null)
            {
                foreach (var userToBlock in usersToBlock)
                {
                    UpdateUser(definitionSettings.SwitchId, userToBlock);
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
        IEnumerable<dynamic> GetSites(int switchId, dynamic telesEnterpriseId)
        {
            return telesEnterpriseManager.GetSites(switchId, telesEnterpriseId);
        }
        IEnumerable<dynamic> GetUsers(int switchId, dynamic siteId)
        {
            return telesEnterpriseManager.GetUsers(switchId, siteId);
        }
        Dictionary<dynamic, dynamic> GetSiteRoutingGroups(int switchId, dynamic siteId)
        {
            return  telesEnterpriseManager.GetSiteRoutingGroups(switchId, siteId);
        }
        void UpdateUser(int switchId, dynamic user)
        {
            telesEnterpriseManager.UpdateUser(switchId, user);
        }
    }
}

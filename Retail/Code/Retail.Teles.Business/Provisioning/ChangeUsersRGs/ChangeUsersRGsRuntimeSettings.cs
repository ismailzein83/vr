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

            var accountExtendedSettings = new AccountBEManager().GetExtendedSettings<InterpriseAccountMappingInfo>(context.AccountBEDefinitionId, context.AccountId);
            if(accountExtendedSettings != null)
            {
                var sites = GetSites(definitionSettings.SwitchId, accountExtendedSettings.TelesEnterpriseId);
                if(sites != null)
                {
                    ChangeUsersRGsAccountState changeUsersRGsAccountState;
                    var usersToBlock = GetUsersToBlockForeachSite(definitionSettings, sites, out changeUsersRGsAccountState);
                    UpdateBlockedUsersState(context.AccountBEDefinitionId, context.AccountId, changeUsersRGsAccountState);
                }
            }
        }
        List<dynamic> GetUsersToBlockForeachSite(ChangeUsersRGsDefinitionSettings definitionSettings, IEnumerable<dynamic> sites, out ChangeUsersRGsAccountState changeUsersRGsAccountState)
        {
            changeUsersRGsAccountState = new ChangeUsersRGsAccountState();
            List<dynamic> usersToBlock = new List<dynamic>();

            foreach(var site in sites)
            {
                 var users = GetUsers(definitionSettings.SwitchId, site.id);
                 Dictionary<dynamic, dynamic> siteRoutingGroups = GetSiteRoutingGroups(definitionSettings.SwitchId, site.id);
                 if (users != null && siteRoutingGroups != null)
                {
                    GetUsersToBlock(definitionSettings, usersToBlock, changeUsersRGsAccountState, siteRoutingGroups, users);
                    UpdateBlockedUsers(definitionSettings, usersToBlock);
                }
            }
            return usersToBlock;
        }
        void GetUsersToBlock(ChangeUsersRGsDefinitionSettings definitionSettings, List<dynamic> usersToBlock, ChangeUsersRGsAccountState changeUsersRGsAccountState, Dictionary<dynamic, dynamic> siteRoutingGroups, IEnumerable<dynamic> users)
        {

            List<dynamic> existingRoutingGroups = null;
            dynamic newRoutingGroup = null;
            foreach (dynamic siteRoutingGroup in siteRoutingGroups.Values)
            {
                if (definitionSettings.ExistingRoutingGroupCondition != null)
                {
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
                        throw new Exception("More than one routing group available for new routing group condition.");
                    newRoutingGroup = siteRoutingGroup.id;
                }
            }
            if (newRoutingGroup == null)
                throw new Exception("No routing group available for new routing group condition.");
            foreach (var user in users)
            {
                if(existingRoutingGroups == null || existingRoutingGroups.Contains(user.routingGroupId))
                {
                    if (definitionSettings.SaveChangesToAccountState)
                    {
                        if (changeUsersRGsAccountState.ChangesByActionType == null)
                            changeUsersRGsAccountState.ChangesByActionType = new Dictionary<string, ChURGsActionCh>();
                        ChURGsActionCh chURGsActionCh;
                        if (!changeUsersRGsAccountState.ChangesByActionType.TryGetValue(definitionSettings.ActionType, out chURGsActionCh))
                        {
                            chURGsActionCh = new ChURGsActionCh();
                            changeUsersRGsAccountState.ChangesByActionType.Add(definitionSettings.ActionType, chURGsActionCh);
                        }
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

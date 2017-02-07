using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var accountExtendedSettings = new AccountBEManager().GetExtendedSettings<EnterpriseAccountMappingInfo>(context.AccountBEDefinitionId, context.AccountId);
            var changeUsersRGsAccountState = new AccountBEManager().GetExtendedSettings<ChangeUsersRGsAccountState>(context.AccountBEDefinitionId, context.AccountId);
            var changedUsers = GetChangedUsers(definitionSettings.ActionType, definitionSettings.SwitchId, changeUsersRGsAccountState);

            RevertBlockedUsers(definitionSettings.SwitchId, changedUsers);
            RevertBlockedUsersState(context.AccountBEDefinitionId, context.AccountId, changeUsersRGsAccountState);
           
        }

        void RevertBlockedUsersState(Guid accountBEDefinition, long accountId, ChangeUsersRGsAccountState changeUsersRGsAccountState)
        {
            if (changeUsersRGsAccountState != null && changeUsersRGsAccountState.ChangesByActionType != null && changeUsersRGsAccountState.ChangesByActionType.Count == 0)
            {
                accountBEManager.DeleteAccountExtendedSetting<ChangeUsersRGsAccountState>(accountBEDefinition, accountId);

            }else
            {
                accountBEManager.UpdateAccountExtendedSetting<ChangeUsersRGsAccountState>(accountBEDefinition, accountId, changeUsersRGsAccountState);
            }
        }
        void RevertBlockedUsers(int switchId, List<dynamic> changedUsers)
        {
            if (changedUsers != null)
            {
                foreach (dynamic changedUser in changedUsers)
                {
                    UpdateUser(switchId, changedUser);
                }
            }
        }
        List<dynamic> GetChangedUsers(string actionType,int switchId, ChangeUsersRGsAccountState changeUsersRGsAccountState)
        {
            List<dynamic> changedUsers = null;
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
                            var user = GetUser(switchId, changesByUser.Key);
                            if(user != null)
                            {
                                user.routingGroupId = changesByUser.Value.OriginalRGId;
                                changedUsers.Add(user);
                            }
                        }
                    }
                    changeUsersRGsAccountState.ChangesByActionType.Remove(actionType);
                }
            }
            return changedUsers;
        }

        dynamic GetUser(int switchId,dynamic userId)
        {
            return telesEnterpriseManager.GetUser(switchId, userId);
        }
        void UpdateUser(int switchId, dynamic user)
        {
            telesEnterpriseManager.UpdateUser(switchId, user);
        }
    }
}

using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Teles.Business.AccountBEActionTypes;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.Teles.Business
{
    public class TelesAccountManager
    {
        AccountBEManager _accountBEManager = new AccountBEManager();

        #region Public Methods
        public UpdateOperationOutput<AccountDetail> UnmapTelesAccount(TelesAccountToUnmap input)
        {
            Account account = _accountBEManager.GetAccount(input.AccountBEDefinitionId, input.AccountId);
            AccountActionDefinition accountActionDefinition = new AccountBEDefinitionManager().GetAccountActionDefinition(input.AccountBEDefinitionId, input.ActionDefinitionId);

            if (accountActionDefinition != null && IsUnmapToAccountValid(account, accountActionDefinition))
            {
                var unmappingTelesAccountActionSettings = accountActionDefinition.ActionDefinitionSettings as UnmappingTelesAccountActionSettings;

                if (account.TypeId.Equals(unmappingTelesAccountActionSettings.CompanyTypeId))
                {
                    return new TelesEnterpriseManager().UnmapEnterpriseToAccount(input);
                }
                else if (account.TypeId.Equals(unmappingTelesAccountActionSettings.SiteTypeId))
                {
                    return new TelesSiteManager().UnmapSiteToAccount(input);
                }
                else if (account.TypeId.Equals(unmappingTelesAccountActionSettings.UserTypeId))
                {
                    return new TelesUserManager().UnmapUserToAccount(input);
                }
            }

            return new UpdateOperationOutput<AccountDetail>
            {
                Result = UpdateOperationResult.Failed
            };
        }

        public bool DoesUserHaveExecutePermission(Guid accountBEDefinitionId)
        {
            var accountDefinitionActions = new AccountBEDefinitionManager().GetAccountActionDefinitions(accountBEDefinitionId);
            foreach (var a in accountDefinitionActions)
            {
                var settings = a.ActionDefinitionSettings as MappingTelesSiteActionSettings;
                if (settings != null)
                    return settings.DoesUserHaveExecutePermission();
            }
            return false;
        }

        #endregion

        #region Private Methods
        private bool IsUnmapToAccountValid(Account account, AccountActionDefinition accountActionDefinition)
        {
            if (accountActionDefinition.ActionDefinitionSettings != null)
            {
                return _accountBEManager.EvaluateAccountCondition(account, accountActionDefinition.AvailabilityCondition);
            }
            return false;
        }


        #endregion
    }
}

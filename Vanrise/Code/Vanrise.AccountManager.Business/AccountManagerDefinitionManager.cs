using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Business;
using Vanrise.Common.Business;
using Vanrise.AccountManager.Entities;
using Vanrise.Common;
using Retail.BusinessEntity.Entities.AccountManager;
using Vanrise.AccountManager.Data;

namespace Vanrise.AccountManager.Business
{
    #region Public Methods
    public class AccountManagerDefinitionManager
    {
       public BusinessEntityDefinition GetAccountManagerDefinition(Guid accountManagerDefinitionId)
       {
           BusinessEntityDefinitionManager manager = new BusinessEntityDefinitionManager();
           return manager.GetBusinessEntityDefinition(accountManagerDefinitionId);
       }
       public AccountManagerBEDefinitionSettings GetAccountManagerDefinitionSettings(Guid accountManagerDefinitionId)
       {
           var accountManagerDefinition = GetAccountManagerDefinition(accountManagerDefinitionId);
           accountManagerDefinition.ThrowIfNull("accountManagerDefinition", accountManagerDefinitionId);
           accountManagerDefinition.Settings.ThrowIfNull("accountManagerDefinition.Settings");
           return accountManagerDefinition.Settings.CastWithValidate<AccountManagerBEDefinitionSettings>("accountManagerDefinition.Settings");
       }
       public IEnumerable<AccountManagerSubViewDefinition> GetAccountManagerSubViewsDefinition(Guid accountManagerDefinitionId)
       {
           var accountManagerDefinitionSettings = GetAccountManagerDefinitionSettings(accountManagerDefinitionId);
           return accountManagerDefinitionSettings.SubViews;
       }
       public IEnumerable<AccountManagerAssignmentConfigs> GetAssignmentDefinitionConfigs()
       {
           ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
           return manager.GetExtensionConfigurations<AccountManagerAssignmentConfigs>(AccountManagerAssignmentConfigs.EXTENSION_TYPE).OrderByDescending(x => x.Name);
       }
       public IEnumerable<AccountManagerSubViewsConfigs> GetSubViewsDefinitionConfigs()
       {
           ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
           return manager.GetExtensionConfigurations<AccountManagerSubViewsConfigs>(AccountManagerSubViewsConfigs.EXTENSION_TYPE).OrderByDescending(x => x.Name);
       }
       public AccountManagerAssignmentRuntime GetAccountManagerAssignmentRuntimeEditor(AccountManagerAssignmentRuntimeInput accountManagerAssignmentRuntimeInput)
       {
           
           AccountManagerAssignmentRuntime accountManagerAssignmentRuntime = new AccountManagerAssignmentRuntime();
           var accountManagerDefinitionSettings = GetAccountManagerDefinitionSettings(accountManagerAssignmentRuntimeInput.AccountManagerDefinitionId);
           var assignmentDefinitions = accountManagerDefinitionSettings.AssignmentDefinitions;
           foreach (var assignmentDefinition in assignmentDefinitions)
           {
             
               if (assignmentDefinition.AccountManagerAssignementDefinitionId == accountManagerAssignmentRuntimeInput.AssignmentDefinitionId)
               {
                   accountManagerAssignmentRuntime.AccountManagrAssignmentDefinition = assignmentDefinition;
               }
           }
           if (accountManagerAssignmentRuntimeInput.AccountManagerAssignementId != null)
           {
               AccountManagerAssignmentManager assignmentManager = new AccountManagerAssignmentManager();
               var accountManagerAssignmentId = accountManagerAssignmentRuntimeInput.AccountManagerAssignementId.Value;
               accountManagerAssignmentRuntime.AccountManagerAssignment = assignmentManager.GetAccountManagerAssignment(accountManagerAssignmentId);

           }
           return accountManagerAssignmentRuntime;
       }

    }
    #endregion
}

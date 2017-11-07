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
       public IEnumerable<AccountManagerAssignmentConfig> GetAssignmentDefinitionConfigs()
       {
           ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
           return manager.GetExtensionConfigurations<AccountManagerAssignmentConfig>(AccountManagerAssignmentConfig.EXTENSION_TYPE).OrderByDescending(x => x.Name);
       }
       public IEnumerable<AccountManagerSubViewsConfig> GetSubViewsDefinitionConfigs()
       {
           ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
           return manager.GetExtensionConfigurations<AccountManagerSubViewsConfig>(AccountManagerSubViewsConfig.EXTENSION_TYPE).OrderByDescending(x => x.Name);
       }
    }
    #endregion
}

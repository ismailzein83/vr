using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Business;
using Vanrise.Common.Business;
using Vanrise.AccountManager.Entities;

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
       public IEnumerable<AccountManagerAssignmentConfigs> GetAssignmentDefinitionConfigs()
       {
           ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
           return manager.GetExtensionConfigurations<AccountManagerAssignmentConfigs>(AccountManagerAssignmentConfigs.EXTENSION_TYPE).OrderByDescending(x => x.Name);
       }

    }
    #endregion
}

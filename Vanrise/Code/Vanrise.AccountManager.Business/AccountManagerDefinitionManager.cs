using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Business;

namespace Vanrise.AccountManager.Business
{
   public class AccountManagerDefinitionManager
    {
       public BusinessEntityDefinition GetAccountManagerDefinition(Guid accountManagerDefinitionId)
       {
           BusinessEntityDefinitionManager manager = new BusinessEntityDefinitionManager();
           return manager.GetBusinessEntityDefinition(accountManagerDefinitionId);
       }
    }
}

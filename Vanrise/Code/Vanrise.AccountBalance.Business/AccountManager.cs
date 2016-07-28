using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class AccountManager
    {
        public dynamic GetAccount(long accountId)
        {
            int accountBEDefinitionId = new ConfigurationManager().GetAccountBEDefinitionId();
            return new GenericData.Business.BusinessEntityManager().GetEntity(accountBEDefinitionId, accountId);
        }
        public AccountInfo GetAccountInfo(long accountId)
        {
            int accountBEDefinitionId = new ConfigurationManager().GetAccountBEDefinitionId();
            return new GenericData.Business.BusinessEntityManager().GetEntityInfo(accountBEDefinitionId,AccountInfo.BEInfoType, accountId);
        }
    }
}

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
            Guid accountBEDefinitionId = new ConfigurationManager().GetAccountBEDefinitionId();
            return new GenericData.Business.BusinessEntityManager().GetEntity(accountBEDefinitionId, accountId);
        }
        public AccountInfo GetAccountInfo(long accountId)
        {
            Guid accountBEDefinitionId = new ConfigurationManager().GetAccountBEDefinitionId();
            return new GenericData.Business.BusinessEntityManager().GetEntityInfo(accountBEDefinitionId,AccountInfo.BEInfoType, accountId);
        }

        AccountTypeManager _accountTypeManager = new AccountTypeManager();
        GenericData.Business.BusinessEntityManager _businessEntityManager = new GenericData.Business.BusinessEntityManager();
        public dynamic GetAccount(Guid accountTypeId, long accountId)
        {
            Guid accountBEDefinitionId = _accountTypeManager.GetAccountBEDefinitionId(accountTypeId);
            return _businessEntityManager.GetEntity(accountBEDefinitionId, accountId);
        }
        public AccountInfo GetAccountInfo(Guid accountTypeId, long accountId)
        {
            Guid accountBEDefinitionId = _accountTypeManager.GetAccountBEDefinitionId(accountTypeId);
            return _businessEntityManager.GetEntityInfo(accountBEDefinitionId, AccountInfo.BEInfoType, accountId);
        }
    }
}

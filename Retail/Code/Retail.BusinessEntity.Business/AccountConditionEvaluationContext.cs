using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountConditionEvaluationContext : IAccountConditionEvaluationContext
    {
        public AccountConditionEvaluationContext()
        {

        }

        Guid _accountBEDefinitionId; 
        long _accountId;
        public AccountConditionEvaluationContext(Guid accountBEDefinitionId, long accountId)
        {
            _accountBEDefinitionId = accountBEDefinitionId;
            _accountId = accountId;
        }

        Account _account;
        public Account Account
        {
            get
            {
                if (_account == null)
                    _account = new AccountBEManager().GetAccount(_accountBEDefinitionId, _accountId);
                return _account;
            }
            set
            {
                _account = value;
            }
        }
    }
}

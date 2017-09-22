using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions
{
    public class RevertStatusPostAction : AccountProvisionPostAction
    {
        public override void ExecutePostAction(IAccountProvisionPostActionContext context)
        {
            List<long> accountIds = new List<long>();
            accountIds.Add(context.AccountId);
            AccountBEManager accountBEManager = new AccountBEManager();
            var childAccountsIds = accountBEManager.GetChildAccountIds(context.AccountBEDefinitionId, context.AccountId, true);
            if (childAccountsIds != null)
                accountIds.AddRange(childAccountsIds);
            throw new NotImplementedException();
        }
    }
}

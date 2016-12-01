using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.Business
{
    public class AccountViewManager
    {
        public List<AccountViewDefinition> GetAccountViews(long accountId)
        {
            var allViews = new ConfigurationManager().GetAccountViewDefinitions();
            var account = new AccountManager().GetAccount(accountId);
            if (account == null)
                throw new NullReferenceException(String.Format("account '{0}'", accountId));
            var accountConditionContext = new AccountConditionEvaluationContext { Account = account };
            return allViews.Where(itm => itm.AvailabilityCondition == null || itm.AvailabilityCondition.Evaluate(accountConditionContext)).ToList();
        }
    }
}

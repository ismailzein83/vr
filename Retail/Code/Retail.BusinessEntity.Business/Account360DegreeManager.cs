using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.Business
{
    public class Account360DegreeManager
    {
        public List<Account360DegreeView> GetAccount360DegreeViews(long accountId)
        {
            VRComponentTypeManager componentTypeManager = new VRComponentTypeManager();
            var allViews = componentTypeManager.GetComponentTypes<Account360DegreeViewSettings, Account360DegreeView>();
            var account = new AccountManager().GetAccount(accountId);
            if (account == null)
                throw new NullReferenceException(String.Format("account '{0}'", accountId));
            var accountConditionContext = new AccountConditionEvaluationContext { Account = account };
            return allViews.Where(itm => itm.Settings.AvailabilityCondition == null || itm.Settings.AvailabilityCondition.Evaluate(accountConditionContext)).ToList();
        }
    }
}

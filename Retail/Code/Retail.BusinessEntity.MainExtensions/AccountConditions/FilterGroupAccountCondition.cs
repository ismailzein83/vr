using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.MainExtensions.AccountConditions
{
    public class FilterGroupAccountCondition : AccountCondition
    {
        public RecordFilterGroup FilterGroup { get; set; }

        static AccountManager s_accountManager = new AccountManager();
        public override bool Evaluate(IAccountConditionEvaluationContext context)
        {
            return s_accountManager.IsAccountMatchWithFilterGroup(context.Account, this.FilterGroup);
        }
    }
}

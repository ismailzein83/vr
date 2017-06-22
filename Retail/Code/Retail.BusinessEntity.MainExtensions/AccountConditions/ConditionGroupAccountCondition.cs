using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountConditions
{
    public enum LogicalOperator
    {
        [Description("AND")]
        And = 0,
        [Description("OR")]
        Or = 1,
    }
    public class ConditionGroupAccountCondition : AccountCondition
    {
        public List<AccountConditionItem> AccountConditionItems { get; set; }
        public LogicalOperator LogicalOperator { get; set; }

        public override Guid ConfigId
        {
            get { return new Guid("FEE1242D-8664-4C64-B203-BAE3290DCF3F"); }
        }

        public override bool Evaluate(IAccountConditionEvaluationContext context)
        {
            if (this.AccountConditionItems != null)
            {
                AccountBEManager accountBEManager = new AccountBEManager();
                bool result = false;
                foreach (var accountConditionItem in this.AccountConditionItems)
                {

                    result = accountBEManager.EvaluateAccountCondition(context.Account, accountConditionItem.AccountCondition);
                    switch (this.LogicalOperator)
                    {
                        case MainExtensions.AccountConditions.LogicalOperator.And:
                            if (!result)
                                return false;
                            break;
                        case MainExtensions.AccountConditions.LogicalOperator.Or:
                            if (result)
                                return true;
                            break;
                    }
                }
            }
            return true;
        }
    }
    public class AccountConditionItem
    {
        public string Name { get; set; }
        public AccountCondition AccountCondition { get; set; }
    }
}

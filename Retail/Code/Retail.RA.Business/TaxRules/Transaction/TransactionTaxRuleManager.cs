using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Rules;

namespace Retail.RA.Business
{
    public class TransactionTaxRuleManager : GenericRuleManager<TransactionTaxRule>
    {
        public void ApplyTaxRule(ITransactionTaxRuleContext context, Guid ruleDefinitionId, GenericRuleTarget target)
        {
            this.ApplyTransactionTaxRule(context, () => GetMatchRule(ruleDefinitionId, target), target);
        }

        public void ApplyTaxRule(ITransactionTaxRuleContext context, RuleTree ruleTree, GenericRuleTarget target)
        {
            this.ApplyTransactionTaxRule(context, () => ruleTree.GetMatchRule(target) as TransactionTaxRule, target);
        }

        void ApplyTransactionTaxRule(ITransactionTaxRuleContext context, Func<TransactionTaxRule> getMatchRule, GenericRuleTarget target)
        {
            var transactionTaxRule = getMatchRule();
            if (transactionTaxRule != null)
            {
                transactionTaxRule.Settings.ApplyTransactionTaxRule(context);
                context.Rule = transactionTaxRule;
            }
        }
    }
}

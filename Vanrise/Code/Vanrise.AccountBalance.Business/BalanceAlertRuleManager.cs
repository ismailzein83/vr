using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class BalanceAlertRuleManager : GenericRuleManager<BalanceAlertRule>
    {
        public BalanceAlertRule GetMatchRule(dynamic account)
        {
            var configurationManager = new ConfigurationManager();
            var balanceAlertConfig = configurationManager.GetBalanceAlertConfig();
            var accountBEDefinitionId = configurationManager.GetAccountBEDefinitionId();
            GenericRuleTarget ruleTarget = BuildRuleTarget(balanceAlertConfig.RuleDefinitionId, balanceAlertConfig, accountBEDefinitionId, account);
            return base.GetMatchRule(balanceAlertConfig.RuleDefinitionId, ruleTarget);
        }

        private GenericRuleTarget BuildRuleTarget(int ruleDefinitionId, BalanceAlertConfig balanceAlertConfig, int accountBEDefinitionId, dynamic account)
        {
            GenericRuleTarget ruleTarget = new GenericRuleTarget
            {
                EffectiveOn = DateTime.Now,
                TargetFieldValues = new Dictionary<string, object>()
            };
            var ruleDefinition = GetRuleDefinition(ruleDefinitionId);
            var accountPropertyEvaluatorContext = new BEPropertyEvaluatorContext(accountBEDefinitionId, account);
            foreach (var criteriaField in ruleDefinition.CriteriaDefinition.Fields)
            {
                AccountRuleCriteriaSetting criteriaSetting;
                if (!balanceAlertConfig.CriteriaSettingsByCriteriaName.TryGetValue(criteriaField.FieldName, out criteriaSetting))
                    throw new NullReferenceException(String.Format("criteriaSetting. CriteriaName '{0}'", criteriaField.FieldName));
                if (criteriaSetting.Evaluator == null)
                    throw new NullReferenceException(String.Format("criteriaSetting.Evaluator. CriteriaName '{0}'", criteriaField.FieldName));

                ruleTarget.TargetFieldValues.Add(criteriaField.FieldName, criteriaSetting.Evaluator.GetPropertyValue(accountPropertyEvaluatorContext));
            }
            return ruleTarget;
        }

        private GenericRuleDefinition GetRuleDefinition(int ruleDefinitionId)
        {
            var ruleDefinition = new GenericRuleDefinitionManager().GetGenericRuleDefinition(ruleDefinitionId);
            if (ruleDefinition == null)
                throw new NullReferenceException(String.Format("ruleDefinition. Id '{0}'", ruleDefinitionId));
            if (ruleDefinition.CriteriaDefinition == null)
                throw new NullReferenceException(String.Format("ruleDefinition.CriteriaDefinition. Id '{0}'", ruleDefinitionId));
            if (ruleDefinition.CriteriaDefinition.Fields == null)
                throw new NullReferenceException(String.Format("ruleDefinition.CriteriaDefinition.Fields. Id '{0}'", ruleDefinitionId));
            return ruleDefinition;
        }
    }
}

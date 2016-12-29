using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Business;

namespace Vanrise.BusinessProcess.WFActivities
{
    public sealed class ExecuteRules : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<IRuleTarget>> ImportedDataToValidate { get; set; }

        [RequiredArgument]
        public InArgument<string> BusinessRulesKey { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<IRuleTarget> importedDataToValidate = this.ImportedDataToValidate.Get(context);
            string businessRulesKey = this.BusinessRulesKey.Get(context);

            List<BPViolatedRule> violatedBusinessRulesByTarget = new List<BPViolatedRule>(); ;
            bool stopExecutionFlag;
            
            BPBusinessRuleDefinitionManager bpBusinessRuleManager = new BPBusinessRuleDefinitionManager();
            List<BPBusinessRuleDefinition> bpBusinessRules = bpBusinessRuleManager.GetBPBusinessRuleDefinitions(businessRulesKey, context.GetSharedInstanceData().InstanceInfo.DefinitionID);

            IEnumerable<BusinessRule> rules = BuildBusinessRules(bpBusinessRules);
            ExecuteValidation(rules, importedDataToValidate, context, violatedBusinessRulesByTarget, out stopExecutionFlag);
            AppendValidationMessages(context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID, context.GetSharedInstanceData().InstanceInfo.ParentProcessID, violatedBusinessRulesByTarget);

            if (stopExecutionFlag)
                throw new InvalidWorkflowException("One or more business rules were not satisfied and led to stop the execution of the worklfow");
        }

        private IEnumerable<BusinessRule> BuildBusinessRules(List<BPBusinessRuleDefinition> bpBusinessRules)
        {
            if (bpBusinessRules == null)
                return null;
            List<BusinessRule> rules = new List<BusinessRule>();
            BPBusinessRuleActionManager bpRuleActionManager = new BPBusinessRuleActionManager();

            foreach (BPBusinessRuleDefinition bpBusinessRule in bpBusinessRules)
            {
                BPBusinessRuleAction action = bpRuleActionManager.GetBusinessRuleAction(bpBusinessRule.BPBusinessRuleDefinitionId);
                BusinessRule rule = new BusinessRule() { Condition = bpBusinessRule.Settings.Condition, Action = action.Details.Settings.Action };
                rules.Add(rule);
            }
            return rules;
        }

        private void ExecuteValidation(IEnumerable<BusinessRule> rules, IEnumerable<IRuleTarget> targets, ActivityContext activityContext, List<BPViolatedRule> violatedBusinessRulesByTarget, out bool stopExecutionFlag)
        {
            stopExecutionFlag = false;
            foreach (BusinessRule rule in rules)
            {
                foreach (IRuleTarget target in targets)
                {
                    if (!rule.Condition.ShouldValidate(target))
                        continue;

                    IBusinessRuleConditionValidateContext validationContext = new BusinessRuleConditionValidateContext(activityContext) { Target = target };
                    if (rule.Condition.Validate(validationContext))
                        continue;


                    BusinessRuleActionExecutionContext actionContext = new BusinessRuleActionExecutionContext() { Target = target };
                    rule.Action.Execute(actionContext);
                    violatedBusinessRulesByTarget.Add(new BPViolatedRule() { Target = target, Rule = rule, Message = validationContext.Message });
                    if (actionContext.StopExecution)
                    {
                        stopExecutionFlag = true;
                        return;
                    }
                }
            }
        }

        private void AppendValidationMessages(long processIntanceId, long? parentProcessId, List<BPViolatedRule> violatedBusinessRulesByTarget)
        {
            List<BPValidationMessage> messages = new List<BPValidationMessage>();
            foreach (BPViolatedRule violatedRule in violatedBusinessRulesByTarget)
            {
                BPValidationMessage msg = new BPValidationMessage()
                {
                    ProcessInstanceId = processIntanceId,
                    ParentProcessId = parentProcessId,
                    TargetKey = violatedRule.Target.Key,
                    TargetType = violatedRule.Target.TargetType,
                    Severity = violatedRule.Rule.Action.GetSeverity(),
                    Message = violatedRule.Message
                };

                messages.Add(msg);
            }

            BPValidationMessageManager manager = new BPValidationMessageManager();
            manager.InsertIntoTrackingTable(messages);
            manager.Insert(messages);

        }
    }
}

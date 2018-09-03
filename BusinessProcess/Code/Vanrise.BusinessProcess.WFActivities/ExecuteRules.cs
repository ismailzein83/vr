using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Business;
using Vanrise.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.WFActivities
{
    public sealed class ExecuteRules : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<IRuleTarget>> ImportedDataToValidate { get; set; }

        [RequiredArgument]
        public InArgument<string> BusinessRulesKey { get; set; }

        public InArgument<bool> WriteMessagesToParentProcess { get; set; }

        public InArgument<string> ParentMessagePrefix { get; set; }

        public InArgument<bool> GetRulesFromParentProcess { get; set; }

        public InArgument<bool> DoNotThrowExceptionOnRulesViolation { get; set; }
        public InArgument<int?> BPBusinessRuleSetId { get; set; }

        public OutArgument<bool> ViolatedRulesExist { get; set; }

        public OutArgument<List<BPValidationMessage>> WarningMessages { get; set; }

        public OutArgument<List<BPValidationMessage>> ErrorMessages { get; set; }

        public OutArgument<bool> NeedsConfirmation { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            List<BPValidationMessage> errorMessages = null;
            List<BPValidationMessage> warningMessages = null;

            IEnumerable<IRuleTarget> importedDataToValidate = this.ImportedDataToValidate.Get(context);
            string businessRulesKey = this.BusinessRulesKey.Get(context);

            bool writeMessagesToParentProcess = this.WriteMessagesToParentProcess.Get(context);
            string parentMessagePrefix = this.ParentMessagePrefix.Get(context);


            List<BPViolatedRule> violatedBusinessRulesByTarget = new List<BPViolatedRule>(); ;
            bool stopExecutionFlag;

            bool getRulesFromParentProcess = this.GetRulesFromParentProcess.Get(context);
            bool doNotThrowExceptionOnRulesViolation = this.DoNotThrowExceptionOnRulesViolation.Get(context);

            BPBusinessRuleDefinitionManager bpBusinessRuleManager = new BPBusinessRuleDefinitionManager();
            List<BPBusinessRuleDefinition> bpBusinessRules = new List<BPBusinessRuleDefinition>();
            Guid rulesDefinitionId;

            if (getRulesFromParentProcess)
            {
                var parentProcessId = context.GetSharedInstanceData().InstanceInfo.ParentProcessID;
                var bpInstanceManager = new BPInstanceManager();
                if (!parentProcessId.HasValue)
                    throw new ArgumentNullException("ParentProcessID not found");

                var parentBPInstance = bpInstanceManager.GetBPInstance(parentProcessId.Value);
                parentBPInstance.ThrowIfNull("Parent BPInstance not found");

                rulesDefinitionId = parentBPInstance.DefinitionID;
            }
            else
                rulesDefinitionId = context.GetSharedInstanceData().InstanceInfo.DefinitionID;

            int? bPBusinessRuleSetId = this.BPBusinessRuleSetId.Get(context);
            bpBusinessRules = bpBusinessRuleManager.GetBPBusinessRuleDefinitions(businessRulesKey, rulesDefinitionId);

            IEnumerable<BusinessRule> rules = BuildBusinessRules(bpBusinessRules, bPBusinessRuleSetId);
            ExecuteValidation(rules, importedDataToValidate, context, violatedBusinessRulesByTarget, out stopExecutionFlag);
            AppendValidationMessages(context, violatedBusinessRulesByTarget, writeMessagesToParentProcess, parentMessagePrefix, out errorMessages, out warningMessages);

            this.WarningMessages.Set(context, warningMessages);
            this.NeedsConfirmation.Set(context, warningMessages.Count > 0);

            if (stopExecutionFlag)
            {
                this.ViolatedRulesExist.Set(context, true);
                this.ErrorMessages.Set(context, errorMessages);
                if (!doNotThrowExceptionOnRulesViolation)
                    throw new VRBusinessException("One or more business rules were not satisfied and led to stop the execution of the worklfow");
            }
        }

        private IEnumerable<BusinessRule> BuildBusinessRules(List<BPBusinessRuleDefinition> bpBusinessRules, int? bPBusinessRuleSetId)
        {
            if (bpBusinessRules == null)
                return null;
            List<BusinessRule> rules = new List<BusinessRule>();
            BPBusinessRuleActionManager bpRuleActionManager = new BPBusinessRuleActionManager();


            foreach (BPBusinessRuleDefinition bpBusinessRule in bpBusinessRules)
            {
                BPBusinessRuleEffectiveAction bPBusinessRuleEffectiveAction = bpRuleActionManager.GetEffectiveRuleAction(bpBusinessRule.BPBusinessRuleDefinitionId, bPBusinessRuleSetId);
                if (!bPBusinessRuleEffectiveAction.Disabled)
                {
                    BusinessRule rule = new BusinessRule()
                    {
                        BPBusinessRuleDefinitionId = bpBusinessRule.BPBusinessRuleDefinitionId,
                        Condition = bpBusinessRule.Settings.Condition,
                        Action = bPBusinessRuleEffectiveAction.Action,
                        ExecutionDependsOnRules = bpBusinessRule.Settings.ExecutionDependsOnRules
                    };
                    rules.Add(rule);
                }
            }
            return rules;
        }

        private void ExecuteValidation(IEnumerable<BusinessRule> rules, IEnumerable<IRuleTarget> targets, ActivityContext activityContext, List<BPViolatedRule> violatedBusinessRulesByTarget, out bool stopExecutionFlag)
        {
            stopExecutionFlag = false;
            List<Guid> violatedRulesThatStopsExecution = new List<Guid>();
            foreach (BusinessRule rule in rules)
            {
                if (violatedRulesThatStopsExecution.Count > 0 &&
                    rule.ExecutionDependsOnRules != null &&
                    rule.ExecutionDependsOnRules.Exists(ruleDefinitionId => violatedRulesThatStopsExecution.Contains(ruleDefinitionId)))
                {
                    violatedRulesThatStopsExecution.Add(rule.BPBusinessRuleDefinitionId);
                    continue;
                }

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
                    }
                }
                if (stopExecutionFlag == true)
                    violatedRulesThatStopsExecution.Add(rule.BPBusinessRuleDefinitionId);
            }
        }

        private void AppendValidationMessages(CodeActivityContext context, List<BPViolatedRule> violatedBusinessRulesByTarget, bool writeMessagesToParentProcess, string parentMessagePrefix, out List<BPValidationMessage> errorMessages, out List<BPValidationMessage> warningMessages)
        {
            errorMessages = new List<BPValidationMessage>();
            warningMessages = new List<BPValidationMessage>();

            long processIntanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            long? parentProcessId = context.GetSharedInstanceData().InstanceInfo.ParentProcessID;

            violatedBusinessRulesByTarget.Sort((x, y) => x.CompareTo(y));

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

                if (msg.Severity == ActionSeverity.Error)
                    errorMessages.Add(msg);
                else if (msg.Severity == ActionSeverity.Warning)
                    warningMessages.Add(msg);

                if (writeMessagesToParentProcess)
                    context.WriteTrackingMessageToParentProcess(MapSeverity(msg.Severity), string.Concat(parentMessagePrefix, msg.Message));

                context.WriteTrackingMessage(MapSeverity(msg.Severity), msg.Message);
            }

            BPValidationMessageManager manager = new BPValidationMessageManager();
            manager.Insert(messages);
        }

        private LogEntryType MapSeverity(ActionSeverity actionSeverity)
        {
            switch (actionSeverity)
            {
                case ActionSeverity.Information:
                    return LogEntryType.Information;
                case ActionSeverity.Warning:
                    return LogEntryType.Warning;
                case ActionSeverity.Error:
                    return LogEntryType.Error;
                default:
                    return LogEntryType.Verbose;
            }
        }
    }
}

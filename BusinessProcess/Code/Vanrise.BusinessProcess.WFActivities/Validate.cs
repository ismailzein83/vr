using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.WFActivities
{
    public sealed class Validate : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<IRuleTarget>> ImportedDataToValidate { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<BusinessRule>> BusinessRules { get; set; }




        protected override void Execute(CodeActivityContext context)
        {
            bool stopExecutionFlag = false;
            IEnumerable<IRuleTarget> importedDataToValidate = this.ImportedDataToValidate.Get(context);
            IEnumerable<BusinessRule> rules = this.BusinessRules.Get(context);
            List<BPViolatedRule> violatedBusinessRulesByTarget = new List<BPViolatedRule>(); ;
            this.ExecuteValidation(rules, importedDataToValidate, violatedBusinessRulesByTarget, ref stopExecutionFlag);
            this.AppendValidationMessages(context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID, context.GetSharedInstanceData().InstanceInfo.ParentProcessID, violatedBusinessRulesByTarget);
            if (stopExecutionFlag)
                throw new InvalidWorkflowException("One or more business rules were not satisfied and led to stop the execution of the worklfow");
        }

        private void ExecuteValidation(IEnumerable<BusinessRule> rules, IEnumerable<IRuleTarget> targets, List<BPViolatedRule> violatedBusinessRulesByTarget, ref bool stopExecutionFlag)
        {

            //foreach (BusinessRule rule in rules)
            //{
            //    foreach (IRuleTarget target in targets)
            //    {
            //        if (rule.Condition.ShouldValidate(target))
            //        {
            //            bool valid = rule.Condition.Validate(target);

            //            if (!valid)
            //            {
            //                BusinessRuleActionExecutionContext actionContext = new BusinessRuleActionExecutionContext() { Target = target };
            //                rule.Action.Execute(actionContext);
            //                violatedBusinessRulesByTarget.Add(new BPViolatedRule() { Target = target, Rule = rule });
            //                if (actionContext.StopExecution)
            //                {
            //                    stopExecutionFlag = true;
            //                    return;
            //                }

            //            }
            //        }
            //    }
            //}
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
                    Message = violatedRule.Rule.Condition.GetMessage(violatedRule.Target)
                };

                messages.Add(msg);
            }

            BPValidationMessageManager manager = new BPValidationMessageManager();
            //manager.InsertIntoTrackingTable(messages);
            manager.Insert(messages);

        }
    }
}

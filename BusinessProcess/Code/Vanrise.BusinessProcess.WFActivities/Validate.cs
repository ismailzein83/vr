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

        private Dictionary<IRuleTarget, BusinessRule> _violatedBusinessRulesByTarget = new Dictionary<IRuleTarget, BusinessRule>();
        private bool _stopExecutionFlag;

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<IRuleTarget> importedDataToValidate = this.ImportedDataToValidate.Get(context);
            IEnumerable<BusinessRule> rules = this.BusinessRules.Get(context);
            
            this.ExecuteValidation(rules, importedDataToValidate);
            this.AppendValidationMessages(context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID, context.GetSharedInstanceData().InstanceInfo.ParentProcessID);

            if (this._stopExecutionFlag)
                throw new InvalidWorkflowException("One or more business rules were not satisfied and led to stop the execution of the worklfow");
        }

        private void ExecuteValidation(IEnumerable<BusinessRule> rules, IEnumerable<IRuleTarget> targets)
        {
            foreach (BusinessRule rule in rules)
            {
                foreach (IRuleTarget target in targets)
                {
                    if (rule.Condition.ShouldValidate(target))
                    {
                        bool valid = rule.Condition.Validate(target);

                        if (!valid)
                        {
                            BusinessRuleActionExecutionContext actionContext = new BusinessRuleActionExecutionContext() { Target = target };
                            rule.Action.Execute(actionContext);
                            if(actionContext.StopExecution)
                                this._stopExecutionFlag = true;

                            this._violatedBusinessRulesByTarget.Add(target, rule);
                        }
                    }
                }
            }
        }

        private void AppendValidationMessages(long processIntanceId, long? parentProcessId)
        {
            List<ValidationMessage> messages = new List<ValidationMessage>();
            foreach (KeyValuePair<IRuleTarget, BusinessRule> kvp in this._violatedBusinessRulesByTarget)
            {
                ValidationMessage msg = new ValidationMessage()
                {
                    ProcessInstanceId = processIntanceId,
                    ParentProcessId = parentProcessId,
                    TargetKey = kvp.Key.Key,
                    TargetType = kvp.Key.TargetType,
                    Severity = kvp.Value.Action.GetSeverity(),
                    Message = kvp.Value.Condition.GetMessage(kvp.Key)
                };

                messages.Add(msg);
            }

            ValidationMessageManager manager = new ValidationMessageManager();
            manager.Insert(messages);
            manager.InsertIntoTrackingTable(messages);
        }
    }
}

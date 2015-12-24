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

        private List<BusinessRule> _violatedBusinessRules = new List<BusinessRule>();
        private bool _stopExecutionFlag;

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<IRuleTarget> importedDataToValidate = this.ImportedDataToValidate.Get(context);
            IEnumerable<BusinessRule> rules = this.BusinessRules.Get(context);
            
            this.ExecuteValidation(rules, context, importedDataToValidate);
            this.AppendValidationMessages();

            if (this._stopExecutionFlag)
                throw new InvalidWorkflowException("One or more business rules were not satisfied and led to stop the execution of the worklfow");
        }

        private void ExecuteValidation(IEnumerable<BusinessRule> rules, CodeActivityContext context, IEnumerable<IRuleTarget> targets)
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
                            this._violatedBusinessRules.Add(rule);
                        }
                    }
                }
            }
        }

        private void AppendValidationMessages()
        {
            foreach (BusinessRule rule in this._violatedBusinessRules)
            {

            }
        }
    }
}

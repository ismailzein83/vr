using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Business;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class ValidateInputData : CodeActivity
    {

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedZone>> ImportedZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedCode>> ImportedCodes { get; set; }

        [RequiredArgument]
        public OutArgument<bool> StopExecution { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ImportedZone> importedZones = this.ImportedZones.Get(context);
            IEnumerable<ImportedCode> importedCodes = this.ImportedCodes.Get(context);

            List<IRuleTarget> targets = new List<IRuleTarget>();
            targets.AddRange(importedCodes);
            targets.AddRange(importedZones);

            this.ExecuteValidation(context, targets);
        }

        private void ExecuteValidation(CodeActivityContext context, IEnumerable<IRuleTarget> targets)
        {
            IEnumerable<BusinessRule> rules = this.GetRulesConfiguration();

            foreach (BusinessRule rule in rules)
            {
                foreach (IRuleTarget target in targets)
                {
                    if (rule.Condition.ShouldValidate(target))
                    {
                        bool valid = rule.Condition.Validate(target);

                        if (!valid)
                        {
                            //TODO: This check should be removed when stop execution functionality is implemented correctly
                            if (rule.Action is StopExecutionAction)
                                this.StopExecution.Set(context, true);

                            rule.Action.Execute(target);
                        }
                    }
                }
            }
        }

        private IEnumerable<BusinessRule> GetRulesConfiguration()
        {
            BusinessRule codeGroupNotFoundRule = new BusinessRule()
            {
                Condition = new CodeGroupCondition(),
                Action = new StopExecutionAction()
            };

            BusinessRule multipleCountriesInSameZoneRule = new BusinessRule()
            {
                Condition = new MultipleCountryCondition(),
                Action = new ExcludeItemAction()
            };

            List<BusinessRule> rules = new List<BusinessRule>();
            rules.Add(codeGroupNotFoundRule);
            rules.Add(multipleCountriesInSameZoneRule);

            return rules;
        }
    }
}

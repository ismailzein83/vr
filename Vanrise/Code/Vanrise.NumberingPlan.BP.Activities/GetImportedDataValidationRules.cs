using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;
using Vanrise.NumberingPlan.Business;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.BP.Activities
{
    public sealed class GetImportedDataValidationRules : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<IEnumerable<BusinessRule>> ImportedDataValidationRules { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<BusinessRule> rules = this.GetRulesConfiguration();
            this.ImportedDataValidationRules.Set(context, rules);
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

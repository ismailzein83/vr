using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Business;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;



namespace TOne.WhS.SupplierPriceList.BP.Activities
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

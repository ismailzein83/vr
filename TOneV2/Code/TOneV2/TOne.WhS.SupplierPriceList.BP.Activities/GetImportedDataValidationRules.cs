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

            BusinessRule missingZones = new BusinessRule()
            {
                Condition = new MissingZonesCondition(),
                Action = new StopExecutionAction()
            };

            BusinessRule missingCodes = new BusinessRule()
            {
                Condition = new MissingCodesCondition(),
                Action = new StopExecutionAction()
            };

            BusinessRule missingRates = new BusinessRule()
            {
                Condition = new MissingRatesCondition(),
                Action = new StopExecutionAction()
            };

            BusinessRule missingBED = new BusinessRule()
            {
                Condition = new MissingBEDCondition(),
                Action = new StopExecutionAction()
            };

            BusinessRule sameCodeInSameZone = new BusinessRule()
            {
                Condition = new SameCodeInSameZoneCondition(),
                Action = new StopExecutionAction()
            };

            BusinessRule sameZoneWithDifferentRates = new BusinessRule()
            {
                Condition = new SameZoneWithDifferentRatesCondition(),
                Action = new StopExecutionAction()
            };

            List<BusinessRule> rules = new List<BusinessRule>();
            rules.Add(missingZones);
            rules.Add(missingCodes);
            rules.Add(missingRates);
            rules.Add(missingBED);
            rules.Add(codeGroupNotFoundRule);
            rules.Add(multipleCountriesInSameZoneRule);
            rules.Add(sameCodeInSameZone);
            rules.Add(sameZoneWithDifferentRates);

            return rules;
        }
    }
}

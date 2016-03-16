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

    public sealed class GetImportedCountriesDataValidationRules : CodeActivity
    {

        [RequiredArgument]
        public OutArgument<IEnumerable<BusinessRule>> ImportedCountriesDataValidationRules { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<BusinessRule> rules = this.GetRulesConfiguration();
            this.ImportedCountriesDataValidationRules.Set(context, rules);
        }

        private IEnumerable<BusinessRule> GetRulesConfiguration()
        {

            BusinessRule sameCodeWithDifferentZones = new BusinessRule()
            {
                Condition = new SameCodeWithDifferentZonesCondition(),
                Action = new StopExecutionAction()
            };


            List<BusinessRule> rules = new List<BusinessRule>();
            rules.Add(sameCodeWithDifferentZones);

            return rules;
        }
    }
}

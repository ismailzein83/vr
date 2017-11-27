using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common.Business;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class ChangesInPastCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as AllCountriesToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ICPParametersContext cpContext = context.GetExtension<ICPParametersContext>();
            if (cpContext.EffectiveDate >= DateTime.Now.Date)
                return true;
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            CountryManager countryManager = new CountryManager();
            AllCountriesToProcess allCountriesToProcess = context.Target as AllCountriesToProcess;
            var invalidCountries = new List<string>();
            foreach (var country in allCountriesToProcess.Countries)
            {

                if (!saleZoneManager.IsCountryEmpty(cpContext.SellingNumberPlanId, country.CountryId, DateTime.Now))
                    invalidCountries.Add(countryManager.GetCountryName(country.CountryId));
            }
            if (invalidCountries.Count > 0)
            {
                context.Message = string.Format("Can not apply changes in the past for a country with existing zones. Violated country(ies): {0}.", string.Join(", ", invalidCountries));
                return false;
            }
            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}

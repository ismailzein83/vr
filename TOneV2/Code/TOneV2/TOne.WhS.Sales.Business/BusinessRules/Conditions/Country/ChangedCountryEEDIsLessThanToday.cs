using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class ChangedCountryEEDIsLessThanToday : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is AllCustomerCountriesToChange;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            var allCountriesToChange = context.Target as AllCustomerCountriesToChange;

            if (allCountriesToChange.CustomerCountriesToChange == null || allCountriesToChange.CustomerCountriesToChange.Count() == 0)
                return true;

            var invalidCountryNames = new List<string>();
            var countryManager = new Vanrise.Common.Business.CountryManager();

            DateTime today = DateTime.Today;

            foreach (CustomerCountryToChange countryToChange in allCountriesToChange.CustomerCountriesToChange)
            {
                if (countryToChange.CloseEffectiveDate < today)
                    invalidCountryNames.Add(countryManager.GetCountryName(countryToChange.CountryId));
            }

            if (invalidCountryNames.Count > 0)
            {
                context.Message = string.Format("Cannot end the following countries before the date of today: {0}", string.Join(", ", invalidCountryNames));
                return false;
            }

            return true;
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}

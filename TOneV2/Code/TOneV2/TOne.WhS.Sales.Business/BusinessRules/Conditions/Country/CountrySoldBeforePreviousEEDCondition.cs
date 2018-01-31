using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class CountrySoldBeforePreviousEEDCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is AllCustomerCountriesToAdd;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            var allCustomerCountriesToAdd = context.Target as AllCustomerCountriesToAdd;

            if (allCustomerCountriesToAdd.CustomerCountriesToAdd == null || allCustomerCountriesToAdd.CustomerCountriesToAdd.Count() == 0)
                return true;

            var violatedCountryNames = new List<string>();
            var countryManager = new Vanrise.Common.Business.CountryManager();

            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();
            Dictionary<int, DateTime> lastCountryEEDByCountryIds = GetLastCountryEEDByCountryId(ratePlanContext.OwnerId, allCustomerCountriesToAdd.CustomerCountriesToAdd.MapRecords(x => x.CountryId));

            if (lastCountryEEDByCountryIds.Count == 0)
                return true;

            foreach (CustomerCountryToAdd countryToAdd in allCustomerCountriesToAdd.CustomerCountriesToAdd)
            {
                DateTime lastCountryEED;

                if (!lastCountryEEDByCountryIds.TryGetValue(countryToAdd.CountryId, out lastCountryEED))
                    continue;

                if (countryToAdd.BED < lastCountryEED)
                    violatedCountryNames.Add(countryManager.GetCountryName(countryToAdd.CountryId));
            }

            if (violatedCountryNames.Count > 0)
            {
                context.Message = string.Format("Can not sell country before its previous End date. Violated country(ies): {0}", string.Join(", ", violatedCountryNames));
                return false;
            }

            return true;
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }

        #region Private Methods
        private Dictionary<int, DateTime> GetLastCountryEEDByCountryId(int customerId, IEnumerable<int> countryIds)
        {
            var lastCountryEEDByCountryIds = new Dictionary<int, DateTime>();
            IEnumerable<CustomerCountry2> customerCountries = new CustomerCountryManager().GetAllCustomerCountriesByCountryIds(customerId, countryIds);

            if (customerCountries != null)
            {
                foreach (CustomerCountry2 customerCountry in customerCountries.OrderByDescending(x => x.BED))
                {
                    if (!customerCountry.EED.HasValue || lastCountryEEDByCountryIds.ContainsKey(customerCountry.CountryId))
                        continue;
                    lastCountryEEDByCountryIds.Add(customerCountry.CountryId, customerCountry.EED.Value);
                }
            }

            return lastCountryEEDByCountryIds;
        }
        #endregion
    }
}

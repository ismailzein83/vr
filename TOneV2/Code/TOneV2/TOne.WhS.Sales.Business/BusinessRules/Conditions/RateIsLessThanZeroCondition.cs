using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class RateIsLessThanZeroCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is AllDataByZone;
        }
        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var allDataByZone = context.Target as AllDataByZone;

            if (allDataByZone.DataByZoneList == null || allDataByZone.DataByZoneList.Count() == 0)
                return true;

            var invalidCountryNames = new HashSet<string>();
            var countryManager = new Vanrise.Common.Business.CountryManager();

            foreach (DataByZone zoneData in allDataByZone.DataByZoneList)
            {
                string countryName = countryManager.GetCountryName(zoneData.CountryId);

                if ((zoneData.IsCustomerCountryNew.HasValue && zoneData.IsCustomerCountryNew.Value) || invalidCountryNames.Contains(countryName))
                    continue;

                if (BusinessRuleUtilities.IsAnyZoneRateNegative(zoneData))
                    invalidCountryNames.Add(countryName);
            }

            if (invalidCountryNames.Count > 0)
            {
                context.Message = string.Format("Rates of following sold country(ies) are negative: {0}", string.Join(", ", invalidCountryNames));
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

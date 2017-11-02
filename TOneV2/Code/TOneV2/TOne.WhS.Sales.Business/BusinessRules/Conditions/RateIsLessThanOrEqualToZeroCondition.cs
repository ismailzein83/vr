using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class RateIsLessThanOrEqualToZeroCondition : BusinessRuleCondition
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

                if (invalidCountryNames.Contains(countryName))
                    continue;

                if ((zoneData.NormalRateToChange != null && zoneData.NormalRateToChange.NormalRate <= 0) || (zoneData.OtherRatesToChange != null && zoneData.OtherRatesToChange.Any(x => x.NormalRate <= 0)))
                {
                    invalidCountryNames.Add(countryName);
                    continue;
                }
            }

            if (invalidCountryNames.Count > 0)
            {
                context.Message = string.Format("The rates of the following countries must be greater than zero: {0}", string.Join(", ", invalidCountryNames));
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

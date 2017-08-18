using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class SetRateInPastForSPCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is ZoneDataByCountryIds;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.Customer || ratePlanContext.IsFirstSellingProductOffer.Value)
                return true;

            var zoneDataByCountryIds = context.Target as ZoneDataByCountryIds;
            var invalidCountryNames = new List<string>();

            foreach (KeyValuePair<int, List<DataByZone>> kvp in zoneDataByCountryIds)
            {
                string countryName = new CountryManager().GetCountryName(kvp.Key);

                foreach (DataByZone zoneData in kvp.Value)
                {
                    if (zoneData.NormalRateToChange != null && zoneData.NormalRateToChange.BED < DateTime.Today)
                    {
                        invalidCountryNames.Add(countryName);
                        break;
                    }

                    if (zoneData.OtherRatesToChange != null)
                    {
                        foreach (RateToChange otherRate in zoneData.OtherRatesToChange)
                        {
                            if (otherRate.BED < DateTime.Today)
                            {
                                invalidCountryNames.Add(countryName);
                                break;
                            }
                        }
                    }
                }
            }

            if (invalidCountryNames.Count > 0)
            {
                context.Message = string.Format("The following countries have zones with rates whose BED is less than today: {0}", string.Join(", ", invalidCountryNames));
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

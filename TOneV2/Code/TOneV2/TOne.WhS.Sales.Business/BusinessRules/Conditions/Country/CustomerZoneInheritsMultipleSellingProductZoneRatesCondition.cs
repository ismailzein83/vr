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
    public class CustomerZoneInheritsMultipleSellingProductZoneRatesCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is CustomerCountryToAdd;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                return true;

            var countryToAdd = context.Target as CustomerCountryToAdd;
            IEnumerable<ExistingZone> countryZones = ratePlanContext.EffectiveAndFutureExistingZonesByCountry.GetRecord(countryToAdd.CountryId);

            if (countryZones == null || countryZones.Count() == 0)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("SaleZones of Country '{0}' were not found", countryToAdd.CountryId));

            DateTime? validCountryBED = null;
            Dictionary<long, DataByZone> dataByZoneId = GetDataByZoneId(ratePlanContext.DataByZoneList);

            foreach (ExistingZone countryZone in countryZones)
            {
                DataByZone zoneData = dataByZoneId.GetRecord(countryZone.ZoneId);

                if (zoneData != null && zoneData.NormalRateToChange != null)
                {
                    if (zoneData.NormalRateToChange.BED == Utilities.Max(countryToAdd.BED, countryZone.BED))
                        continue;
                }

                ZoneInheritedRates zoneRates = ratePlanContext.InheritedRatesByZoneId.GetRecord(countryZone.ZoneId);
                IEnumerable<SaleRate> zoneNormalRates = (zoneRates != null) ? zoneRates.NormalRates : null;

                if (zoneNormalRates == null || zoneNormalRates.Count() == 0)
                    continue;

                if (zoneNormalRates.FindAllRecords(x => !x.EED.HasValue || x.EED.Value > countryToAdd.BED).Count() > 1)
                {
                    DateTime lastRateBED = zoneNormalRates.Last().BED;
                    if (!validCountryBED.HasValue || validCountryBED.Value < lastRateBED)
                        validCountryBED = lastRateBED;
                }
            }

            if (validCountryBED.HasValue)
            {
                var countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(countryToAdd.CountryId);
                string validCountryBEDString = validCountryBED.Value.ToString(ratePlanContext.DateFormat);
                context.Message = string.Format("Sell date of country '{0}' must be greater than or equal to the BED of the last inherited rate '{1}'", countryName, validCountryBEDString);
                return false;
            }

            return true;
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
        #region Private Methods
        private Dictionary<long, DataByZone> GetDataByZoneId(IEnumerable<DataByZone> zoneDataList)
        {
            var dataByZoneId = new Dictionary<long, DataByZone>();

            if (zoneDataList != null)
            {
                foreach (DataByZone zoneData in zoneDataList)
                {
                    if (!dataByZoneId.ContainsKey(zoneData.ZoneId))
                        dataByZoneId.Add(zoneData.ZoneId, zoneData);
                }
            }

            return dataByZoneId;
        }
        #endregion
    }
}

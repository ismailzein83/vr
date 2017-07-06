using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class ConflictingRateCurrenciesCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is CountryData;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            var ratePlanContext = context.GetExtension<IRatePlanContext>();
            if (ratePlanContext.OwnerType == BusinessEntity.Entities.SalePriceListOwnerType.SellingProduct)
                return true;

            var countryData = context.Target as CountryData;

            if (countryData.ZoneDataByZoneId == null || countryData.ZoneDataByZoneId.Count == 0)
                return true;

            IEnumerable<ExistingZone> countryZones = ratePlanContext.EffectiveAndFutureExistingZonesByCountry.GetRecord(countryData.CountryId);
            if (countryZones == null || countryZones.Count() == 0)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("No zones were found for country '{0}'", countryData.CountryId));

            string countryName = new CountryManager().GetCountryName(countryData.CountryId);

            var newNormalRatesByZoneId = new Dictionary<long, RateToChange>();
            var closedNormalRatesByZoneId = new Dictionary<long, RateToClose>();

            if (!AreAllActionsDefinedOnSameDate(countryData.ZoneDataByZoneId.Values, newNormalRatesByZoneId, closedNormalRatesByZoneId))
            {
                context.Message = string.Format("The new and closed rates of country '{0}' must be defined on the same date", countryName);
                return false;
            }

            int countryZonesCount = countryZones.Count();

            if (newNormalRatesByZoneId.Count != countryZonesCount && closedNormalRatesByZoneId.Count != countryZonesCount)
            {
                var saleRateManager = new SaleRateManager();
                int sellingProductId = new CarrierAccountManager().GetSellingProductId(ratePlanContext.OwnerId);

                foreach (ExistingZone countryZone in countryZones)
                {
                    RateToChange newNormalRate = newNormalRatesByZoneId.GetRecord(countryZone.ZoneId);
                    RateToClose closedNormalRate = closedNormalRatesByZoneId.GetRecord(countryZone.ZoneId);

                    if (newNormalRate != null)
                        continue;

                    int zoneRateCurrencyId;

                    if (closedNormalRate != null)
                    {
                        SaleEntityZoneRate spZoneRate = ratePlanContext.ActionRateLocator.GetSellingProductZoneRate(sellingProductId, countryZone.ZoneId);
                        if (spZoneRate == null || spZoneRate.Rate == null)
                            throw new DataIntegrityValidationException(string.Format("No rate was found for zone '{0}' of selling product '{1}' on '{2}'", countryZone.ZoneId, sellingProductId, UtilitiesManager.GetDateTimeAsString(closedNormalRate.CloseEffectiveDate)));
                        zoneRateCurrencyId = saleRateManager.GetCurrencyId(spZoneRate.Rate);
                    }
                    else
                    {
                        SaleEntityZoneRate effectiveZoneRate = ratePlanContext.RateLocator.GetSellingProductZoneRate(sellingProductId, countryZone.ZoneId);
                        if (effectiveZoneRate == null || effectiveZoneRate.Rate == null)
                            throw new DataIntegrityValidationException(string.Format("No rate was found for zone '{0}' of selling product '{1}' on '{2}'", countryZone.ZoneId, sellingProductId, UtilitiesManager.GetDateTimeAsString(ratePlanContext.EffectiveDate)));
                        zoneRateCurrencyId = saleRateManager.GetCurrencyId(effectiveZoneRate.Rate);
                    }

                    if (zoneRateCurrencyId != ratePlanContext.CurrencyId)
                    {
                        context.Message = string.Format("The normal rates of the zones of country '{0}' must all have the same currency", countryName);
                        return false;
                    }
                }
            }

            return true;
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }

        #region Private Methods

        private bool AreAllActionsDefinedOnSameDate(IEnumerable<DataByZone> dataByZone, Dictionary<long, RateToChange> newNormalRatesByZoneId, Dictionary<long, RateToClose> closedNormalRatesByZoneId)
        {
            DateTime? actionDate = null;

            foreach (DataByZone zoneData in dataByZone)
            {
                if (zoneData.NormalRateToChange != null)
                {
                    if (!actionDate.HasValue)
                        actionDate = zoneData.NormalRateToChange.BED;
                    else if (zoneData.NormalRateToChange.BED != actionDate.Value)
                        return false;

                    if (!newNormalRatesByZoneId.ContainsKey(zoneData.ZoneId))
                        newNormalRatesByZoneId.Add(zoneData.ZoneId, zoneData.NormalRateToChange);
                }
                else if (zoneData.NormalRateToClose != null)
                {
                    if (!actionDate.HasValue)
                        actionDate = zoneData.NormalRateToClose.CloseEffectiveDate;
                    else if (zoneData.NormalRateToClose.CloseEffectiveDate != actionDate.Value)
                        return false;

                    if (!closedNormalRatesByZoneId.ContainsKey(zoneData.ZoneId))
                        closedNormalRatesByZoneId.Add(zoneData.ZoneId, zoneData.NormalRateToClose);
                }
            }

            return true;
        }

        #endregion
    }
}

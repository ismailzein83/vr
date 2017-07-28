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
            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                return true;

            CountryData countryData = context.Target as CountryData;

            Dictionary<long, RateToChange> newRatesByZoneId;
            Dictionary<long, RateToClose> closedRatesByZoneId;
            DateTime? minRateActionDate;

            if (!DoRateActionsExist(countryData.ZoneDataByZoneId, out newRatesByZoneId, out closedRatesByZoneId, out minRateActionDate))
                return true;

            string countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(countryData.CountryId);

            if (closedRatesByZoneId.Count > 0 && ratePlanContext.CurrencyId != ratePlanContext.SystemCurrencyId)
            {
                context.Message = string.Format("The currency of the process doesn't match the currecny of the system");
                return false;
            }

            IEnumerable<ExistingZone> existingZones = ratePlanContext.ExistingZonesByCountry.GetRecord(countryData.CountryId);
            IEnumerable<ExistingZone> effectiveOrFutureZones = GetEffectiveOrFutureZones(existingZones, countryName, countryData.CountryBED);

            foreach (ExistingZone countryZone in effectiveOrFutureZones)
            {
                RateToChange newRate = newRatesByZoneId.GetRecord(countryZone.ZoneId);
                RateToClose closedRate = closedRatesByZoneId.GetRecord(countryZone.ZoneId);

                DateTime? zoneRateActionDate = null;

                if (newRate != null)
                {
                    if (newRate.BED == minRateActionDate.Value)
                        continue;
                    zoneRateActionDate = newRate.BED;
                }
                else if (closedRate != null)
                {
                    if (closedRate.CloseEffectiveDate == minRateActionDate.Value)
                        continue;
                    zoneRateActionDate = closedRate.CloseEffectiveDate;
                }

                DateTime? xDate = Max(countryZone.BED, countryData.CountryBED, minRateActionDate.Value);
                DateTime? yDate = Min(countryZone.EED, countryData.CountryEED, zoneRateActionDate);

                IEnumerable<ExistingRate> effectiveOrFutureRates = GetEffectiveOrFutureRates(countryZone.ExistingRates, countryData.CountryBED);

                if (AreZoneRatesInConflict(effectiveOrFutureRates, xDate.Value, yDate, ratePlanContext))
                {
                    context.Message = string.Format("All rates of zones for country '{0}' must have same currency", countryName);
                    return false;
                }
            }

            return true;
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }

        #region Private Methods

        private bool DoRateActionsExist(Dictionary<long, DataByZone> dataByZoneId, out Dictionary<long, RateToChange> newRatesByZoneId, out Dictionary<long, RateToClose> closedRatesByZoneId, out DateTime? minRateActionDate)
        {
            if (dataByZoneId == null || dataByZoneId.Count == 0)
            {
                newRatesByZoneId = null;
                closedRatesByZoneId = null;
                minRateActionDate = null;
                return false;
            }

            newRatesByZoneId = new Dictionary<long, RateToChange>();
            closedRatesByZoneId = new Dictionary<long, RateToClose>();
            minRateActionDate = null;

            foreach (DataByZone zoneData in dataByZoneId.Values)
            {
                if (zoneData.NormalRateToChange != null)
                {
                    if (!minRateActionDate.HasValue || zoneData.NormalRateToChange.BED < minRateActionDate.Value)
                        minRateActionDate = zoneData.NormalRateToChange.BED;
                    newRatesByZoneId.Add(zoneData.ZoneId, zoneData.NormalRateToChange);
                }
                else if (zoneData.NormalRateToClose != null)
                {
                    if (!minRateActionDate.HasValue || zoneData.NormalRateToClose.CloseEffectiveDate < minRateActionDate.Value)
                        minRateActionDate = zoneData.NormalRateToClose.CloseEffectiveDate;
                    closedRatesByZoneId.Add(zoneData.ZoneId, zoneData.NormalRateToClose);
                }
            }

            return minRateActionDate.HasValue;
        }

        private IEnumerable<ExistingZone> GetEffectiveOrFutureZones(IEnumerable<ExistingZone> existingZones, string countryName, DateTime countryBED)
        {
            if (existingZones == null || existingZones.Count() == 0)
                throw new VRBusinessException(string.Format("The existing zones of country '{0}' were not found", countryName));

            IEnumerable<ExistingZone> effectiveOrFutureZones = existingZones.FindAllRecords(x => x.IsEffectiveOrFuture(countryBED));

            if (effectiveOrFutureZones.Count() == 0)
                throw new VRBusinessException(string.Format("No effective or future zones of country '{0}' were found after the country's BED '{1}'", countryName, UtilitiesManager.GetDateTimeAsString(countryBED)));

            return effectiveOrFutureZones;
        }

        private IEnumerable<ExistingRate> GetEffectiveOrFutureRates(IEnumerable<ExistingRate> existingRates, DateTime countryBED)
        {
            if (existingRates == null || existingRates.Count() == 0)
                return null;
            return existingRates.FindAllRecords(x => x.IsEffectiveOrFuture(countryBED));
        }

        private bool AreZoneRatesInConflict(IEnumerable<ExistingRate> existingRates, DateTime xDate, DateTime? yDate, IRatePlanContext ratePlanContext)
        {
            if (existingRates == null || existingRates.Count() == 0)
            {
                if (AreProcessAndSystemCurrenciesInConflict(ratePlanContext))
                    return true;
            }
            else
            {
                var saleRateManager = new SaleRateManager();
                DateTime? previousRateEED = null;

                int existingRatesCount = existingRates.Count();

                for (int i = 0; i < existingRatesCount; i++)
                {
                    ExistingRate existingRate = existingRates.ElementAt(i);

                    if (i == 0)
                    {
                        if (existingRate.BED > xDate && AreProcessAndSystemCurrenciesInConflict(ratePlanContext))
                            return true;
                    }
                    else if (existingRate.BED > previousRateEED.Value && AreProcessAndSystemCurrenciesInConflict(ratePlanContext))
                        return true;

                    int existingRateCurrencyId = saleRateManager.GetCurrencyId(existingRate.RateEntity);

                    if (existingRateCurrencyId != ratePlanContext.CurrencyId)
                        return true;

                    if (!existingRate.EED.HasValue || (yDate.HasValue && existingRate.EED.Value >= yDate.Value))
                        break;

                    if (i == existingRatesCount - 1)
                    {
                        if (AreProcessAndSystemCurrenciesInConflict(ratePlanContext))
                            return false;
                    }

                    previousRateEED = existingRate.EED;
                }
            }

            return false;
        }

        private bool AreProcessAndSystemCurrenciesInConflict(IRatePlanContext ratePlanContext)
        {
            return ratePlanContext.CurrencyId != ratePlanContext.SystemCurrencyId;
        }

        private DateTime? Max(params DateTime?[] dates)
        {
            if (dates == null || dates.Count() == 0)
                return default(DateTime);
            else
            {
                int datesCount = dates.Count();
                DateTime? firstDate = dates.ElementAt(0);

                if (datesCount == 1)
                    return firstDate;
                else
                {
                    DateTime? maxDate = firstDate;

                    for (int i = 1; i < dates.Count(); i++)
                    {
                        DateTime? date = dates.ElementAt(i);

                        if (!date.HasValue)
                            continue;

                        if (!maxDate.HasValue || date.Value > maxDate.Value)
                            maxDate = date;
                    }

                    return maxDate;
                }
            }
        }

        private DateTime? Min(params DateTime?[] dates)
        {
            if (dates == null || dates.Count() == 0)
                return default(DateTime);
            else
            {
                int datesCount = dates.Count();
                DateTime? firstDate = dates.ElementAt(0);

                if (datesCount == 1)
                    return firstDate;
                else
                {
                    DateTime? maxDate = firstDate;

                    for (int i = 1; i < dates.Count(); i++)
                    {
                        DateTime? date = dates.ElementAt(i);

                        if (!date.HasValue)
                            continue;

                        if (!maxDate.HasValue || date.Value < maxDate.Value)
                            maxDate = date;
                    }

                    return maxDate;
                }
            }
        }

        #endregion
    }
}

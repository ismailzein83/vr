using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
    public class RateBulkAction : BulkActionType
    {
        #region Fields / Constructors

        private Dictionary<int, DateTime> _datesByCountry;

        private int? _sellingProductId;

        public RateBulkAction()
        {
        }

        #endregion

        public RateCalculationMethod RateCalculationMethod { get; set; }

        public DateTime? BED { get; set; }

        public List<RateSource> RateSources { get; set; }

        #region Bulk Action Members

        public override Guid ConfigId
        {
            get { return new Guid("A893F3C6-D4BF-4C60-BA7D-2A773791D7BD"); }
        }

        public override void ValidateZone(IZoneValidationContext context)
        {
            ZoneItem contextZoneItem = context.GetContextZoneItem(context.ZoneId);

            if (contextZoneItem == null)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("ZoneItem of Zone '{0}' was not found", context.ZoneId));

            if (context.ValidationResult == null)
            {
                context.ValidationResult = new RateBulkActionValidationResult();
            }

            var validationResult = context.ValidationResult as RateBulkActionValidationResult;

            var rateCalculationContext = new RateCalculationMethodContext(context.GetCostCalculationMethodIndex) { ZoneItem = contextZoneItem };
            RateCalculationMethod.CalculateRate(rateCalculationContext);

            var validationResultType = RateBulkActionValidationResultType.Valid;
            List<InvalidZoneRate> targetInvalidRates = null;

            decimal? roundedCalculatedRate = null;

            if (!rateCalculationContext.Rate.HasValue)
            {
                validationResultType = RateBulkActionValidationResultType.DoesNotExist;
                targetInvalidRates = validationResult.EmptyRates;
            }
            else
            {
                roundedCalculatedRate = context.GetRoundedRate(rateCalculationContext.Rate.Value);

                if (roundedCalculatedRate == 0)
                {
                    validationResultType = RateBulkActionValidationResultType.Zero;
                    targetInvalidRates = validationResult.ZeroRates;
                }
                else if (roundedCalculatedRate < 0)
                {
                    validationResultType = RateBulkActionValidationResultType.Negative;
                    targetInvalidRates = validationResult.NegativeRates;
                }
                else if (contextZoneItem.CurrentRate.HasValue && context.GetRoundedRate(contextZoneItem.CurrentRate.Value) == roundedCalculatedRate)
                {
                    validationResultType = RateBulkActionValidationResultType.EqualsCurrentNormalRate;
                    targetInvalidRates = validationResult.DuplicateRates;
                }
            }

            if (validationResultType != RateBulkActionValidationResultType.Valid)
            {
                validationResult.InvalidDataExists = true;
                validationResult.ExcludedZoneIds.Add(context.ZoneId);

                targetInvalidRates.Add(new InvalidZoneRate()
                {
                    ZoneId = contextZoneItem.ZoneId,
                    ZoneName = contextZoneItem.ZoneName,
                    CurrentRate = contextZoneItem.CurrentRate,
                    CalculatedRate = roundedCalculatedRate,
                    ValidationResultType = validationResultType
                });
            }

            context.ValidationResult = validationResult;
        }

        public override bool IsApplicableToCountry(IBulkActionApplicableToCountryContext context)
        {
            return true;
        }

        public override bool IsApplicableToZone(IActionApplicableToZoneContext context)
        {
            if (context.SaleZone.EED.HasValue)
                return false;

            if (BED.HasValue && context.SaleZone.BED > BED)
                return false;

            if (context.OwnerType == SalePriceListOwnerType.Customer)
            {
                if (!_sellingProductId.HasValue)
                {
                    _sellingProductId = new RatePlanManager().GetSellingProductId(context.OwnerId, DateTime.Today, false);
                }

                if (UtilitiesManager.CustomerZoneHasPendingClosedNormalRate(context.OwnerId, _sellingProductId.Value, context.SaleZone.SaleZoneId, context.GetCustomerZoneRate))
                    return false;

                if (_datesByCountry == null)
                {
                    _datesByCountry = UtilitiesManager.GetDatesByCountry(context.OwnerId, DateTime.Today, true);
                }

                if (BED.HasValue && !UtilitiesManager.IsCustomerZoneCountryApplicable(context.SaleZone.CountryId, BED.Value, _datesByCountry))
                    return false;

                if (RateSources != null && RateSources.Count > 0)
                {
                    SaleEntityZoneRate currentCustomerZoneRate = context.GetCustomerZoneRate(context.OwnerId, _sellingProductId.Value, context.SaleZone.SaleZoneId, false);

                    if (currentCustomerZoneRate == null)
                        return false;

                    if (!UtilitiesManager.RateSourcesContain(currentCustomerZoneRate, RateSources))
                        return false;
                }

            }

            return true;
        }

        public override void ApplyBulkActionToZoneItem(IApplyBulkActionToZoneItemContext context)
        {
            var newRates = new List<DraftRateToChange>();

            if (context.ZoneDraft != null && context.ZoneDraft.NewRates != null)
            {
                IEnumerable<DraftRateToChange> newOtherRates = context.ZoneDraft.NewRates.FindAllRecords(x => x.RateTypeId.HasValue);
                newRates.AddRange(newOtherRates);
            }

            var newNormalRate = new DraftRateToChange()
            {
                ZoneId = context.ZoneItem.ZoneId,
                RateTypeId = null
            };

            ZoneItem zoneItem = context.GetContextZoneItem(context.ZoneItem.ZoneId);

            var rateCalculationContext = new RateCalculationMethodContext(context.GetCostCalculationMethodIndex) { ZoneItem = zoneItem };
            RateCalculationMethod.CalculateRate(rateCalculationContext);

            newNormalRate.Rate = context.GetRoundedRate(rateCalculationContext.Rate.Value);
            newNormalRate.BED = GetNewNormalRateBED(zoneItem.CurrentRate, newNormalRate.Rate, zoneItem.ZoneBED, context.OwnerType, zoneItem.CountryId, context.NewRateDayOffset, context.IncreasedRateDayOffset, context.DecreasedRateDayOffset);

            newRates.Add(newNormalRate);
            context.ZoneItem.NewRates = newRates;
        }

        public override void ApplyBulkActionToZoneDraft(IApplyBulkActionToZoneDraftContext context)
        {
            ZoneItem contextZoneItem = context.GetZoneItem(context.ZoneDraft.ZoneId);

            var rateCalculationContext = new RateCalculationMethodContext(context.GetCostCalculationMethodIndex) { ZoneItem = contextZoneItem };
            RateCalculationMethod.CalculateRate(rateCalculationContext);

            decimal roundedNewNormalRateValue = context.GetRoundedRate(rateCalculationContext.Rate.Value);

            AddNewNormalRate(context.OwnerType, context.OwnerId, contextZoneItem, roundedNewNormalRateValue, context.ZoneDraft,context.NewRateDayOffset,context.IncreasedRateDayOffset,context.DecreasedRateDayOffset);
        }

        public override void ApplyCorrectedData(IApplyCorrectedDataContext context)
        {
            var correctedData = context.CorrectedData.CastWithValidate<RateBulkActionCorrectedData>("RateBulkActionCorrectedData");
            correctedData.ZoneCorrectedRates.ThrowIfNull("RateBulkActionCorrectedData.ZoneCorrectedRates");

            foreach (ZoneCorrectedRate zoneCorrectedRate in correctedData.ZoneCorrectedRates)
            {
                ZoneChanges zoneDraft = context.GetZoneDraft(zoneCorrectedRate.ZoneId);
                ZoneItem contextZoneItem = context.GetContextZoneItem(zoneCorrectedRate.ZoneId);
                decimal roundedCorrectedRateValue = context.GetRoundedRate(zoneCorrectedRate.CorrectedRate);
                AddNewNormalRate(context.OwnerType, context.OwnerId, contextZoneItem, roundedCorrectedRateValue, zoneDraft,context.NewRateDayOffset,context.IncreasedRateDayOffset,context.DecreasedRateDayOffset);
            }
        }

        #endregion

        #region Private Methods

        private void AddNewNormalRate(SalePriceListOwnerType ownerType, int ownerId, ZoneItem zoneItem, decimal roundedNewNormalRateValue, ZoneChanges zoneDraft, int newRateDayOffset, int increasedRateDayOffset, int decreasedRateDayOffset)
        {
            var newRates = new List<DraftRateToChange>();

            if (zoneDraft.NewRates != null)
            {
                IEnumerable<DraftRateToChange> newOtherRates = zoneDraft.NewRates.FindAllRecords(x => x.RateTypeId.HasValue);
                newRates.AddRange(newOtherRates);
            }

            var newNormalRate = new DraftRateToChange()
            {
                ZoneId = zoneDraft.ZoneId,
                RateTypeId = null,
                Rate = roundedNewNormalRateValue
            };

            newNormalRate.BED = GetNewNormalRateBED(zoneItem.CurrentRate, newNormalRate.Rate, zoneItem.ZoneBED, ownerType, zoneItem.CountryId, newRateDayOffset, increasedRateDayOffset, decreasedRateDayOffset);

            newRates.Add(newNormalRate);
            zoneDraft.NewRates = newRates;
        }

       private DateTime GetNewNormalRateBED(decimal? currentRate, decimal newRate, DateTime zoneBED, SalePriceListOwnerType ownerType, int countryId, int newRateDayOffset,int increasedRateDayOffset,int decreasedRateDayOffset)
       {
            if (BED.HasValue)
                return BED.Value;

            DateTime newNormalRateBED;
            DateTime todayPlusOffset = GetTodayPlusOffset(currentRate, newRate, newRateDayOffset, increasedRateDayOffset, decreasedRateDayOffset);
            newNormalRateBED = Utilities.Max(todayPlusOffset, zoneBED);

            if (ownerType == SalePriceListOwnerType.Customer)
            {
                DateTime countryBED = _datesByCountry.GetRecord(countryId);
                newNormalRateBED = Utilities.Max(newNormalRateBED, countryBED);
            }

            return newNormalRateBED;
        }

       private DateTime GetTodayPlusOffset(decimal? currentRate, decimal newRate, int newRateDayOffset, int increasedRateDayOffset, int decreasedRateDayOffset)
        {
            DateTime today = DateTime.Today;

            if (!currentRate.HasValue)
                return today.AddDays(newRateDayOffset);
            else
            {
                if (newRate > currentRate.Value)
                    return today.AddDays(increasedRateDayOffset);
                else if (newRate < currentRate.Value)
                    return today.AddDays(decreasedRateDayOffset);
            }

            return today;
        }

        #endregion
    }
}

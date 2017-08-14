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

namespace TOne.WhS.Sales.MainExtensions
{
    public class RateBulkAction : BulkActionType
    {
        #region Fields / Constructors

        private Dictionary<int, DateTime> _datesByCountry;

        private int? _sellingProductId;

        private int _newRateDayOffset;
        private int _increasedRateDayOffset;
        private int _decreasedRateDayOffset;

        public RateBulkAction()
        {
            var configManager = new TOne.WhS.Sales.Business.ConfigManager();
            _newRateDayOffset = configManager.GetNewRateDayOffset();
            _increasedRateDayOffset = configManager.GetIncreasedRateDayOffset();
            _decreasedRateDayOffset = configManager.GetDecreasedRateDayOffset();
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
                    ZoneName = contextZoneItem.ZoneName,
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
            newNormalRate.BED = GetNewNormalRateBED(zoneItem.CurrentRate, newNormalRate.Rate, zoneItem.ZoneBED, context.OwnerType, zoneItem.CountryId);

            newRates.Add(newNormalRate);
            context.ZoneItem.NewRates = newRates;
        }

        public override void ApplyBulkActionToZoneDraft(IApplyBulkActionToZoneDraftContext context)
        {
            var newRates = new List<DraftRateToChange>();

            if (context.ZoneDraft != null && context.ZoneDraft.NewRates != null)
            {
                IEnumerable<DraftRateToChange> newOtherRates = context.ZoneDraft.NewRates.FindAllRecords(x => x.RateTypeId.HasValue);
                newRates.AddRange(newOtherRates);
            }

            var newNormalRate = new DraftRateToChange()
            {
                ZoneId = context.ZoneDraft.ZoneId,
                RateTypeId = null,
            };

            ZoneItem zoneItem = context.GetZoneItem(context.ZoneDraft.ZoneId);

            var rateCalculationContext = new RateCalculationMethodContext(context.GetCostCalculationMethodIndex) { ZoneItem = zoneItem };
            RateCalculationMethod.CalculateRate(rateCalculationContext);

            newNormalRate.Rate = context.GetRoundedRate(rateCalculationContext.Rate.Value);
            newNormalRate.BED = GetNewNormalRateBED(zoneItem.CurrentRate, newNormalRate.Rate, zoneItem.ZoneBED, context.OwnerType, zoneItem.CountryId);

            newRates.Add(newNormalRate);
            context.ZoneDraft.NewRates = newRates;
        }

        public override void ApplyBulkActionToDefaultDraft(IApplyBulkActionToDefaultDraftContext context)
        {

        }

        #endregion

        #region Private Methods

        private DateTime GetNewNormalRateBED(decimal? currentRate, decimal newRate, DateTime zoneBED, SalePriceListOwnerType ownerType, int countryId)
        {
            if (BED.HasValue)
                return BED.Value;

            DateTime newNormalRateBED;

            DateTime todayPlusOffset = GetTodayPlusOffset(currentRate, newRate);
            newNormalRateBED = Utilities.Max(todayPlusOffset, zoneBED);

            if (ownerType == SalePriceListOwnerType.Customer)
            {
                DateTime countryBED = _datesByCountry.GetRecord(countryId);
                newNormalRateBED = Utilities.Max(newNormalRateBED, countryBED);
            }

            return newNormalRateBED;
        }

        private DateTime GetTodayPlusOffset(decimal? currentRate, decimal newRate)
        {
            DateTime today = DateTime.Today;

            if (!currentRate.HasValue)
                return today.AddDays(_newRateDayOffset);
            else
            {
                if (newRate > currentRate.Value)
                    return today.AddDays(_increasedRateDayOffset);
                else if (newRate < currentRate.Value)
                    return today.AddDays(_decreasedRateDayOffset);
            }

            return today;
        }

        #endregion
    }
}

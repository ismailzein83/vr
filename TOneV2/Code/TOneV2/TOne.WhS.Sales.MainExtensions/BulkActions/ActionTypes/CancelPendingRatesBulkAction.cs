using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.MainExtensions
{
    public class CancelPendingRatesBulkAction : BulkActionType
    {
        #region Fields
        private int? _sellingProductId = null;
        #endregion

        #region Bulk Action Members
        public override Guid ConfigId
        {
            get { return new Guid("923550F7-4E24-4182-B12A-366365A7EE54"); }
        }
        public override void ValidateZone(IZoneValidationContext context)
        {

        }
        public override bool IsApplicableToCountry(IBulkActionApplicableToCountryContext context)
        {
            return UtilitiesManager.IsActionApplicableToCountry(context, this.IsApplicableToZone);
        }
        public override bool IsApplicableToZone(IActionApplicableToZoneContext context)
        {
            if (context.OwnerType == SalePriceListOwnerType.SellingProduct)
                throw new Vanrise.Entities.VRBusinessException("Cannot apply the cancel pending rates bulk action on the zones of a selling product");

            if (context.SaleZone.EED.HasValue)
                return false;

            DateTime? countryEED = context.GetCountryEED(context.SaleZone.CountryId);

            if (countryEED.HasValue)
                return false;

            DateTime today = DateTime.Today;

            if (context.SaleZone.BED > today)
                return false;

            DateTime? countryBED = context.GetCountryBED(context.SaleZone.CountryId);
            if (!countryBED.HasValue || countryBED.Value > today)
                return false;

            if (!_sellingProductId.HasValue)
                _sellingProductId = new CarrierAccountManager().GetSellingProductId(context.OwnerId);

            SaleEntityZoneRate effectiveRate = context.GetCustomerZoneRate(context.OwnerId, _sellingProductId.Value, context.SaleZone.SaleZoneId, false);

            if (effectiveRate == null)
                throw new Vanrise.Entities.VRBusinessException(string.Format("The effective rate of zone '{0}' was not found", context.SaleZone.Name));

            if (effectiveRate.Source == SalePriceListOwnerType.Customer)
            {
                if (!effectiveRate.Rate.EED.HasValue)
                    return false;
                else if (effectiveRate.Rate.EED.Value > today)
                    return true;
            }

            SaleEntityZoneRate futureRate = context.GetCustomerZoneRate(context.OwnerId, _sellingProductId.Value, context.SaleZone.SaleZoneId, true);
            if (futureRate == null || futureRate.Source == SalePriceListOwnerType.SellingProduct)
                return false;

            return true;
        }
        public override void ApplyBulkActionToZoneItem(IApplyBulkActionToZoneItemContext context)
        {
            ZoneItem contextZoneItem = context.GetContextZoneItem(context.ZoneItem.ZoneId);
            DraftRateToChange newNormalRate = GetNewNormalRate(contextZoneItem);
            context.ZoneItem.NewRates = GetNewRates(contextZoneItem.NewRates, newNormalRate);
        }
        public override void ApplyBulkActionToZoneDraft(IApplyBulkActionToZoneDraftContext context)
        {
            ZoneItem contextZoneItem = context.GetZoneItem(context.ZoneDraft.ZoneId);
            DraftRateToChange newNormalRate = GetNewNormalRate(contextZoneItem);
            context.ZoneDraft.NewRates = GetNewRates(contextZoneItem.NewRates, newNormalRate);
        }
        #endregion

        #region Private Methods
        private DraftRateToChange GetNewNormalRate(ZoneItem zoneItem)
        {
            return new DraftRateToChange()
            {
                ZoneId = zoneItem.ZoneId,
                RateTypeId = null,
                Rate = zoneItem.CurrentRate.Value,
                BED = DateTime.Today,
                IsCancellingRate = true
            };
        }
        private IEnumerable<DraftRateToChange> GetNewRates(IEnumerable<DraftRateToChange> existingNewRates, DraftRateToChange newNormalRate)
        {
            var newRates = new List<DraftRateToChange>();
            if (existingNewRates != null)
            {
                IEnumerable<DraftRateToChange> newOtherRates = existingNewRates.FindAllRecords(x => x.RateTypeId.HasValue);
                newRates.AddRange(newOtherRates);
            }
            newRates.Add(newNormalRate);
            return newRates;
        }
        #endregion
    }
}

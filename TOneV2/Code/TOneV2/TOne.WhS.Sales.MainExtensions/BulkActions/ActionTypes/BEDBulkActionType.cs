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
    public class BEDBulkActionType : BulkActionType
    {
        #region Fields

        private Dictionary<int, DateTime> _datesByCountry;

        private int? _sellingProductId;

        #endregion

        public override Guid ConfigId
        {
            get { return new Guid("310EAF9D-68B5-466A-9CC4-96121B03A5FD"); }
        }

        public DateTime BED { get; set; }

        public override void ValidateZone(IZoneValidationContext context)
        {

        }

        public override bool IsApplicableToCountry(IBulkActionApplicableToCountryContext context)
        {
            var bulkActionApplicableToAnyCountryZoneInput = new BulkActionApplicableToAnyCountryZoneInput()
            {
                CountryId = context.Country.CountryId,
                OwnerSellingNumberPlanId = context.OwnerSellingNumberPlanId,
                OwnerType = context.OwnerType,
                OwnerId = context.OwnerId,
                ZoneDraftsByZoneId = context.ZoneDraftsByZoneId,
                GetSellingProductZoneRate = context.GetSellingProductZoneRate,
                GetCustomerZoneRate = context.GetCustomerZoneRate,
                GetRateBED = context.GetRateBED,
                IsBulkActionApplicableToZone = IsApplicableToZone
            };
            return UtilitiesManager.IsBulkActionApplicableToAnyCountryZone(bulkActionApplicableToAnyCountryZoneInput);
        }

        public override bool IsApplicableToZone(IActionApplicableToZoneContext context)
        {
            if (context.SaleZone.EED.HasValue)
                return false;

            if (context.ZoneDraft == null || context.ZoneDraft.NewRates == null || !context.ZoneDraft.NewRates.Any(x => !x.RateTypeId.HasValue))
                return false;

            if (context.SaleZone.BED > BED)
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
                    _datesByCountry = UtilitiesManager.GetDatesByCountry(context.OwnerId, DateTime.Today, false);
                }

                if (!UtilitiesManager.IsCustomerZoneCountryApplicable(context.SaleZone.CountryId, BED, _datesByCountry))
                    return false;
            }

            return true;
        }

        public override void ApplyBulkActionToZoneItem(IApplyBulkActionToZoneItemContext context)
        {
            DraftRateToChange newNormalRate = GetZoneNewNormalRate(context.ZoneDraft);
            newNormalRate.BED = BED;
        }

        public override void ApplyBulkActionToZoneDraft(IApplyBulkActionToZoneDraftContext context)
        {
            DraftRateToChange newNormalRate = GetZoneNewNormalRate(context.ZoneDraft);
            newNormalRate.BED = BED;
        }

        #region Private Methods

        private DraftRateToChange GetZoneNewNormalRate(ZoneChanges zoneDraft)
        {
            return zoneDraft.NewRates.FindRecord(x => !x.RateTypeId.HasValue);
        }

        #endregion
    }
}

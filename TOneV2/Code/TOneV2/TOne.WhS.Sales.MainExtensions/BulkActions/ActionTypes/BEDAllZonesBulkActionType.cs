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
    public class BEDAllZonesBulkActionType : BulkActionType
    {
        #region Fields

        private Dictionary<int, DateTime> _datesByCountry;

        private int? _sellingProductId;

        #endregion

        public DateTime BED { get; set; }

        #region Bulk Action Members

        public override Guid ConfigId
        {
            get { return new Guid("F16718B8-8F3B-4455-8440-71A8DFD8F782"); }
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
            if (context.SaleZone.EED.HasValue)
                return false;

            var countryEEd = context.GetCountryEED(context.SaleZone.CountryId);
            if (countryEEd.HasValue)
                return false;

            if (context.SaleZone.BED > BED)
                return false;

            if (context.OwnerType == SalePriceListOwnerType.Customer)
            {
                if (!_sellingProductId.HasValue)
                    _sellingProductId = new RatePlanManager().GetSellingProductId(context.OwnerId, DateTime.Today, false);

                if (_datesByCountry == null)
                    _datesByCountry = UtilitiesManager.GetDatesByCountry(context.OwnerId, DateTime.Today, false);

                if (!UtilitiesManager.IsCustomerZoneCountryApplicable(context.SaleZone.CountryId, BED, _datesByCountry))
                    return false;
            }
            return true;
        }

        public override void ApplyBulkActionToZoneItem(IApplyBulkActionToZoneItemContext context)
        {
            DraftRateToChange newNormalRate = GetZoneNewNormalRate(context.ZoneDraft);
            if (newNormalRate != null)
                newNormalRate.BED = BED;
            else
            {
                SaleRate ownerRate = context.GetCustomerNormalRate(context.OwnerId, context.ZoneItem.ZoneId, BED);
                var newRates = new List<DraftRateToChange>();
                newRates.Add(
                     new DraftRateToChange
                     {
                         ZoneId = context.ZoneItem.ZoneId,
                         RateTypeId = null,
                         BED = BED,
                         Rate = ownerRate.Rate
                     });
                context.ZoneItem.NewRates = newRates;
            }
        }

        public override void ApplyBulkActionToZoneDraft(IApplyBulkActionToZoneDraftContext context)
        {
            DraftRateToChange newNormalRate = GetZoneNewNormalRate(context.ZoneDraft);
            if (newNormalRate != null)
                newNormalRate.BED = BED;
            else
            {
                SaleRate customerRate = context.GetCustomerNormalRate(context.OwnerId, context.ZoneDraft.ZoneId, BED);
                var newRates = new List<DraftRateToChange>();
                newRates.Add(
                     new DraftRateToChange
                     {
                         ZoneId = context.ZoneDraft.ZoneId,
                         RateTypeId = null,
                         BED = BED,
                         Rate = customerRate.Rate
                     });
                context.ZoneDraft.NewRates = newRates;
            }
        }
        #endregion

        #region Private Methods

        private DraftRateToChange GetZoneNewNormalRate(ZoneChanges zoneDraft)
        {
            if (zoneDraft != null && zoneDraft.NewRates != null && zoneDraft.NewRates.Count() > 0)
                return zoneDraft.NewRates.FindRecord(x => !x.RateTypeId.HasValue);
            return null;
        }

        #endregion
    }
}

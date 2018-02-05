using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Entities
{
    public class ZoneItem : IBaseRates
    {
        public long ZoneId { get; set; }
        public string ZoneName { get; set; }
        public int CountryId { get; set; }
        public bool IsCountryNew { get; set; }
        public bool IsCountryEnded { get; set; }
        public DateTime? CountryBED { get; set; }
        public DateTime ZoneBED { get; set; }
        public DateTime? ZoneEED { get; set; }
        public bool IsZonePendingClosed
        {
            get
            {
                return ZoneEED.HasValue;
            }
        }
        public bool IsFutureZone { get; set; }

        #region Rate
        public long? CurrentRateId { get; set; }
        public Decimal? CurrentRate { get; set; }
        
        /// <summary>
        /// CurrentRate currency
        /// </summary>
        public int TargetCurrencyId { get; set; }
        
        /// <summary>
        /// Initial rate currency
        /// </summary>
        public int? CurrentRateCurrencyId { get; set; }
        public string CurrentRateCurrencySymbol { get; set; }
        public DateTime? CurrentRateBED { get; set; }
        public DateTime? CurrentRateEED { get; set; }
        public DateTime? CurrentRateNewEED { get; set; }
        public bool? IsCurrentRateEditable { get; set; }
        public List<RateType> RateTypes { get; set; }
        public Dictionary<int, OtherRate> CurrentOtherRates { get; set; }
        public Decimal? NewRate
        {
            get
            {
                DraftRateToChange newNormalRate = NewRates.FindRecord(x => x.RateTypeId == null);
                if (newNormalRate != null)
                    return newNormalRate.Rate;
                return null;
            }
        }
        public bool? IsNewRateCancelling
        {
            get
            {
                if (NewRate.HasValue)
                {
                    DraftRateToChange newNormalRate = NewRates.FindRecord(x => !x.RateTypeId.HasValue);
                    return newNormalRate.IsCancellingRate;
                }
                return null;
            }
        }
        public IEnumerable<DraftRateToChange> NewRates { get; set; }
        public DateTime? NewRateBED
        {
            get
            {
                DraftRateToChange newNormalRate = NewRates.FindRecord(x => x.RateTypeId == null);
                if (newNormalRate != null)
                    return newNormalRate.BED;
                return null;
            }
        }
        public DateTime? NewRateEED
        {
            get
            {
                DraftRateToChange newNormalRate = NewRates.FindRecord(x => x.RateTypeId == null);
                if (newNormalRate != null)
                    return newNormalRate.EED;
                return null;
            }
        }
        public IEnumerable<DraftRateToClose> ClosedRates { get; set; }
        public decimal? CalculatedRate { get; set; }
        public FutureRate FutureNormalRate { get; set; }
        public Dictionary<int, FutureRate> FutureOtherRates { get; set; }
        #endregion

        #region Routing Product
        public int? CurrentRoutingProductId { get; set; }
        public string CurrentRoutingProductName { get; set; }
        public DateTime? CurrentRoutingProductBED { get; set; }
        public DateTime? CurrentRoutingProductEED { get; set; }
        public bool? IsCurrentRoutingProductEditable { get; set; }
        public DraftNewSaleZoneRoutingProduct NewRoutingProduct { get; set; }
        public DraftChangedSaleZoneRoutingProduct ResetRoutingProduct { get; set; }
        public int? EffectiveRoutingProductId { get; set; }
        public string EffectiveRoutingProductName { get; set; }
        #endregion

        #region Route Options
        public RPRouteDetail RPRouteDetail { get; set; }
        public List<decimal?> Costs { get; set; }
        #endregion

        #region Service
        public IEnumerable<int> CurrentServiceIds { get; set; }
        public IEnumerable<int> EffectiveServiceIds { get; set; }
        public int? CurrentServiceId { get; set; }
        public List<ZoneService> CurrentServices { get; set; }
        public DateTime? CurrentServiceBED { get; set; }
        public DateTime? CurrentServiceEED { get; set; }
        public bool? IsCurrentServiceEditable { get; set; }
        public DraftNewZoneService NewService { get; set; }
        public DraftClosedZoneService ClosedService { get; set; }
        public DraftResetZoneService ResetService { get; set; }
        public IEnumerable<ZoneService> EffectiveServices { get; set; }
        #endregion

        #region Margin
        public decimal? Margin { get; set; }
        public decimal? MarginPercentage { get; set; }
        #endregion

        public decimal? ProfitPerc { get; set; }

        #region IBaseRates Implementation

        public void SetNormalRateBED(DateTime beginEffectiveDate)
        {
            if (!CurrentRateBED.HasValue)
                return;
            CurrentRateBED = beginEffectiveDate;
        }

        public void SetOtherRateBED(int rateTypeId, DateTime beginEffectiveDate)
        {
            if (CurrentOtherRates == null)
                return;
            OtherRate currentOtherRate;
            if (CurrentOtherRates.TryGetValue(rateTypeId, out currentOtherRate))
                currentOtherRate.BED = beginEffectiveDate;
        }

        #endregion
    }

    public class OtherRate
    {
        public int RateTypeId { get; set; }
        public decimal Rate { get; set; }
        public int CurrencyId { get; set; }
        public bool IsRateEditable { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }

    public class FutureRate
    {
        public int? RateTypeId { get; set; }
        public decimal Rate { get; set; }
        public bool IsRateEditable { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
        public RateChangeType RateChange { get; set; }
    }

    public class RateType
    {
        public int RateTypeId { get; set; }
        public string Name { get; set; }
    }
}

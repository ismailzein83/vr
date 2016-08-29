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

namespace TOne.WhS.Sales.Business
{
    public class ZoneRateSetter
    {
        #region Fields

        private SalePriceListOwnerType _ownerType;
        private int _ownerId;
        private int? _sellingProductId;
        private DateTime _effectiveOn;

        private IEnumerable<DraftRateToChange> _newRates;
        private IEnumerable<DraftRateToClose> _rateChanges;

        private int _targetCurrencyId;

        private SaleEntityZoneRateLocator _rateLocator;
        private SaleEntityZoneRateLocator _futureRateLocator;
        private CurrencyExchangeRateManager _currencyExchangeRateManager;
        private SaleRateManager _saleRateManager;
        
        #endregion

        #region Public Methods

        public ZoneRateSetter(SalePriceListOwnerType ownerType, int ownerId, int? sellingProductId, DateTime effectiveOn, Changes changes, int targetCurrencyId)
        {
            _ownerType = ownerType;
            _ownerId = ownerId;
            _sellingProductId = sellingProductId;
            _effectiveOn = effectiveOn;

            if (changes != null && changes.ZoneChanges != null)
            {
                _newRates = changes.ZoneChanges.Where(x => x.NewRates != null).SelectMany(x => x.NewRates);
                _rateChanges = changes.ZoneChanges.Where(x => x.ClosedRates != null).SelectMany(x => x.ClosedRates);
            }

            _targetCurrencyId = targetCurrencyId;

            _rateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(_effectiveOn));
            _futureRateLocator = new SaleEntityZoneRateLocator(new FutureSaleRateReadWithCache());
            _currencyExchangeRateManager = new CurrencyExchangeRateManager();
            _saleRateManager = new SaleRateManager();
        }

        public void SetZoneRate(ZoneItem zoneItem)
        {
            SaleEntityZoneRate rate = (_ownerType == SalePriceListOwnerType.SellingProduct) ?
                _rateLocator.GetSellingProductZoneRate(_ownerId, zoneItem.ZoneId) :
                _rateLocator.GetCustomerZoneRate(_ownerId, (int)_sellingProductId, zoneItem.ZoneId);

            if (rate != null)
            {
                if (rate.Rate != null)
                {
                    zoneItem.CurrentRateId = rate.Rate.SaleRateId;
                    zoneItem.CurrentRate = GetConvertedRate(rate.Rate);
                    zoneItem.CurrentRateBED = rate.Rate.BED;
                    zoneItem.CurrentRateEED = rate.Rate.EED;
                    zoneItem.IsCurrentRateEditable = (rate.Source == _ownerType);
                }

                if (rate.RatesByRateType.Count > 0)
                {
                    zoneItem.CurrentOtherRates = new Dictionary<int, OtherRate>();
                    foreach (KeyValuePair<int, SaleRate> kvp in rate.RatesByRateType)
                    {
                        zoneItem.CurrentOtherRates.Add(kvp.Key, new OtherRate()
                        {
                            Rate = GetConvertedRate(kvp.Value),
                            BED = kvp.Value.BED,
                            EED = kvp.Value.EED
                        });
                    }
                }
            }

            SetZoneFutureNormalRate(zoneItem);
            SetZoneRateChanges(zoneItem);
        }

        private void SetZoneFutureNormalRate(ZoneItem zoneItem)
        {
            SaleEntityZoneRate futureRate = (_ownerType == SalePriceListOwnerType.SellingProduct) ?
                _futureRateLocator.GetSellingProductZoneRate(_ownerId, zoneItem.ZoneId) :
                _futureRateLocator.GetCustomerZoneRate(_ownerId, (int)_sellingProductId, zoneItem.ZoneId);

            if (futureRate != null && futureRate.Rate != null)
            {
                zoneItem.FutureNormalRate = new FutureRate()
                {
                    RateTypeId = futureRate.Rate.RateTypeId,
                    Rate = futureRate.Rate.NormalRate,
                    BED = futureRate.Rate.BED,
                    EED = futureRate.Rate.EED
                };
            }
        }

        #endregion

        #region Private Methods

        private void SetZoneRateChanges(ZoneItem zoneItem)
        {
            zoneItem.NewRates = _newRates.FindAllRecords(x => x.ZoneId == zoneItem.ZoneId);
            zoneItem.ClosedRates = _rateChanges.FindAllRecords(x => x.ZoneId == zoneItem.ZoneId);

            DraftRateToClose rateChange = _rateChanges.FindRecord(itm => itm.RateId == zoneItem.CurrentRateId); // What if currentRateId = null?
            if (rateChange != null)
                zoneItem.CurrentRateNewEED = rateChange.EED;
        }

        private decimal GetConvertedRate(SaleRate saleRate)
        {
            return _currencyExchangeRateManager.ConvertValueToCurrency(saleRate.NormalRate, _saleRateManager.GetCurrencyId(saleRate), _targetCurrencyId, _effectiveOn);
        }
        
        #endregion
    }
}

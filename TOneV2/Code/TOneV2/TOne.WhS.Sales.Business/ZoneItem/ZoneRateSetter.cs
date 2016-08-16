using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class ZoneRateSetter
    {
        SalePriceListOwnerType _ownerType;
        int _ownerId;
        int? _sellingProductId;
        DateTime _effectiveOn;

        IEnumerable<DraftRateToChange> _newRates;
        IEnumerable<DraftRateToClose> _rateChanges;

        public ZoneRateSetter(SalePriceListOwnerType ownerType, int ownerId, int? sellingProductId, DateTime effectiveOn, Changes changes)
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
        }

        public void SetZoneRate(ZoneItem zoneItem)
        {
            SaleEntityZoneRateLocator rateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(_effectiveOn));

            SaleEntityZoneRate rate = (_ownerType == SalePriceListOwnerType.SellingProduct) ?
                rateLocator.GetSellingProductZoneRate(_ownerId, zoneItem.ZoneId) :
                rateLocator.GetCustomerZoneRate(_ownerId, (int)_sellingProductId, zoneItem.ZoneId);

            if (rate != null)
            {
                zoneItem.CurrentRateId = rate.Rate.SaleRateId;
                zoneItem.CurrentRate = rate.Rate.NormalRate;
                zoneItem.CurrentRateBED = rate.Rate.BED;
                zoneItem.CurrentRateEED = rate.Rate.EED;
                zoneItem.IsCurrentRateEditable = (rate.Source == _ownerType);

                if (rate.RatesByRateType.Count > 0)
                {
                    zoneItem.CurrentOtherRates = new Dictionary<int, OtherRate>();
                    foreach (KeyValuePair<int, SaleRate> kvp in rate.RatesByRateType)
                    {
                        zoneItem.CurrentOtherRates.Add(kvp.Key, new OtherRate()
                        {
                            Rate = kvp.Value.NormalRate,
                            BED = kvp.Value.BED,
                            EED = kvp.Value.EED
                        });
                    }
                }
            }

            SetZoneRateChanges(zoneItem);
        }

        void SetZoneRateChanges(ZoneItem zoneItem)
        {
            zoneItem.NewRates = _newRates.FindAllRecords(x => x.ZoneId == zoneItem.ZoneId);
            
            DraftRateToClose rateChange = _rateChanges.FindRecord(itm => itm.RateId == zoneItem.CurrentRateId); // What if currentRateId = null?
            if (rateChange != null)
                zoneItem.CurrentRateNewEED = rateChange.EED;
        }
    }
}

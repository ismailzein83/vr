using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.Business
{
    public class ReadRatesToAddChanges : ISaleRateReader
    {
        private SaleRatesByOwner _allSaleRatesByOwner;

        public ReadRatesToAddChanges(IEnumerable<RateToAdd> allRates, DateTime effectiveDate)
        {
            _allSaleRatesByOwner = GetAllSaleRates(allRates, effectiveDate);
        }

        public SaleRatesByZone GetZoneRates(SalePriceListOwnerType ownerType, int ownerId)
        {
            if (_allSaleRatesByOwner == null)
                return null;

            var saleRateByOwnerType = ownerType == SalePriceListOwnerType.Customer
                ? _allSaleRatesByOwner.SaleRatesByCustomer
                : _allSaleRatesByOwner.SaleRatesByProduct;

            return saleRateByOwnerType == null ? null : saleRateByOwnerType.GetRecord(ownerId);
        }
        private SaleRatesByOwner GetAllSaleRates(IEnumerable<RateToAdd> allRates, DateTime effectiveDate)
        {
            SaleRatesByOwner result = new SaleRatesByOwner
            {
                SaleRatesByCustomer = new VRDictionary<int, SaleRatesByZone>(),
                SaleRatesByProduct = new VRDictionary<int, SaleRatesByZone>()
            };
            SaleRatesByZone saleRateByZone;
            SaleRatePriceList saleRatePriceList;
            foreach (var rateToAdd in allRates)
            {
                if(rateToAdd.PriceListToAdd == null)
                    throw new Exception(string.Format("Rate To Add on zone {0} without Pricelist assigned", rateToAdd.ZoneName));

                VRDictionary<int, SaleRatesByZone> saleRatesByOwner =
                    rateToAdd.PriceListToAdd.OwnerType == SalePriceListOwnerType.SellingProduct ? result.SaleRatesByProduct : result.SaleRatesByCustomer;

                saleRateByZone = saleRatesByOwner.GetOrCreateItem(rateToAdd.PriceListToAdd.OwnerId);
                var existingRateCreationTime = rateToAdd.AddedRates.Any() ? rateToAdd.AddedRates.Last() : null;
                if (existingRateCreationTime == null)
                    throw new Exception(string.Format("Opening a rate without added rates for zone {0}", rateToAdd.ZoneName));
                var existingZoneIdAtRateCreationTime = existingRateCreationTime.AddedZone;
                if (existingZoneIdAtRateCreationTime == null)
                    throw new Exception(string.Format("Opening a rate without added zones for zone {0}", rateToAdd.ZoneName));
                saleRatePriceList = saleRateByZone.GetOrCreateItem(existingZoneIdAtRateCreationTime.ZoneId);
                saleRatePriceList.Rate = GetSaleRateFromRateToAdd(rateToAdd, existingRateCreationTime.RateId, existingZoneIdAtRateCreationTime.ZoneId, effectiveDate);
            }

            return result;
        }

        private SaleRate GetSaleRateFromRateToAdd(RateToAdd rateToAdd, long rateId, long zoneId, DateTime effectiveDate)
        {
            return new SaleRate
            {
                SaleRateId = rateId,
                ZoneId = zoneId,
                Rate = rateToAdd.Rate,
                CurrencyId = rateToAdd.PriceListToAdd.CurrencyId,
                BED = effectiveDate,
                RateChange = RateChangeType.New
            };
        }
    }
}

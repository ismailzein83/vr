using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleEntityZoneRateLocator
    {
        #region ctor/Local Variables
        ISaleRateReader _reader;
        SalePriceListManager _salePriceListManager;
        #endregion

        #region Public Methods
        public SaleEntityZoneRateLocator(ISaleRateReader reader)
        {
            _reader = reader;
            _salePriceListManager = new SalePriceListManager();
        }
        public SaleEntityZoneRate GetCustomerZoneRate(int customerId, int sellingProductId, long saleZoneId)
        {
            SaleEntityZoneRate customerZoneRate;
            if (!HasRate(SalePriceListOwnerType.Customer, customerId, saleZoneId, out customerZoneRate))
                HasRate(SalePriceListOwnerType.SellingProduct, sellingProductId, saleZoneId, out customerZoneRate);
            return customerZoneRate;
        }
        public SaleEntityZoneRate GetSellingProductZoneRate(int sellingProductId, long saleZoneId)
        {
            SaleEntityZoneRate customerZoneRate;
            HasRate(SalePriceListOwnerType.SellingProduct, sellingProductId, saleZoneId, out customerZoneRate);
            return customerZoneRate;
        }
        #endregion

        #region Private Members
        private bool HasRate(SalePriceListOwnerType ownerType, int ownerId, long saleZoneId, out SaleEntityZoneRate saleEntityZoneRate)
        {
            var zoneRates = _reader.GetZoneRates(ownerType, ownerId);
            SaleRatePriceList saleRatePriceList;
            if (zoneRates != null && zoneRates.TryGetValue(saleZoneId, out saleRatePriceList))
            {
                saleEntityZoneRate = new SaleEntityZoneRate
                {
                    Source = ownerType,
                    Rate = saleRatePriceList.Rate,
                    RatesByRateType = saleRatePriceList.RatesByRateType
                };
                return true;
            }
            else
            {
                saleEntityZoneRate = null;
                return false;
            }
        }
        #endregion
     
    }
}

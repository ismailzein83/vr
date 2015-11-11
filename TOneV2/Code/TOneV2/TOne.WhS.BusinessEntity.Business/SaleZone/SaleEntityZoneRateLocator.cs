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
        ISaleRateReader _reader;
        SalePriceListManager _salePriceListManager;

        public SaleEntityZoneRateLocator(ISaleRateReader reader)
        {
            _reader = reader;
            _salePriceListManager = new SalePriceListManager();
        }

        public SaleEntityZoneRate GetCustomerZoneRate(int customerId, int sellingProductId, long saleZoneId)
        {
            SaleEntityZoneRate customerZoneRate;
            if (!HasCustomerRate(customerId, saleZoneId, out customerZoneRate))
                HasProductRate(sellingProductId, saleZoneId, out customerZoneRate);
            if (customerZoneRate != null)
                customerZoneRate.PriceList = _salePriceListManager.GetPriceList(customerZoneRate.Rate.PriceListId);
            return customerZoneRate;
        }

        public SaleEntityZoneRate GetSellingProductZoneRate(int sellingProductId, long saleZoneId)
        {
            SaleEntityZoneRate customerZoneRate;
            HasProductRate(sellingProductId, saleZoneId, out customerZoneRate);
            if (customerZoneRate != null)
                customerZoneRate.PriceList = _salePriceListManager.GetPriceList(customerZoneRate.Rate.PriceListId);
            return customerZoneRate;
        }
        
        private bool HasCustomerRate(int customerId, long saleZoneId, out SaleEntityZoneRate customerZoneRate)
        {
            var zoneRates = _reader.GetZoneRates(SalePriceListOwnerType.Customer, customerId);
            SaleRate saleRate;
            if (zoneRates != null && zoneRates.TryGetValue(saleZoneId, out saleRate))
            {
                customerZoneRate = new SaleEntityZoneRate
                {
                    Rate = saleRate,
                    Source = SalePriceListOwnerType.Customer
                };
                return true;
            }
            else
            {
                customerZoneRate = null;
                return false;
            }
        }

        private bool HasProductRate(int sellingProductId, long saleZoneId, out SaleEntityZoneRate customerZoneRate)
        {
            var zoneRates = _reader.GetZoneRates(SalePriceListOwnerType.SellingProduct, sellingProductId);
            SaleRate saleRate;
            if (zoneRates != null && zoneRates.TryGetValue(saleZoneId, out saleRate))
            {
                customerZoneRate = new SaleEntityZoneRate
                {
                    Rate = saleRate,
                    Source = SalePriceListOwnerType.SellingProduct
                };
                return true;
            }
            else
            {
                customerZoneRate = null;
                return false;
            }
        }
    }
}

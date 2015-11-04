using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CustomerZoneRateLocator
    {
        ISaleRateReader _reader;
        SalePriceListManager _salePriceListManager;

        public CustomerZoneRateLocator(ISaleRateReader reader)
        {
            _reader = reader;
            _salePriceListManager = new SalePriceListManager();
        }

        public CustomerZoneRate GetCustomerZoneRate(int customerId, int sellingProductId, long saleZoneId)
        {
            CustomerZoneRate customerZoneRate;
            if (!HasCustomerRate(customerId, saleZoneId, out customerZoneRate))
                HasProductRate(sellingProductId, saleZoneId, out customerZoneRate);
            if (customerZoneRate != null)
                customerZoneRate.PriceList = _salePriceListManager.GetPriceList(customerZoneRate.Rate.PriceListId);
            return customerZoneRate;
        }
        
        private bool HasCustomerRate(int customerId, long saleZoneId, out CustomerZoneRate customerZoneRate)
        {
            var zoneRates = _reader.GetZoneRates(SalePriceListOwnerType.Customer, customerId);
            SaleRate saleRate;
            if (zoneRates != null && zoneRates.TryGetValue(saleZoneId, out saleRate))
            {
                customerZoneRate = new CustomerZoneRate
                {
                    Rate = saleRate,
                    Source = CustomerZoneRateSource.Customer
                };
                return true;
            }
            else
            {
                customerZoneRate = null;
                return false;
            }
        }

        private bool HasProductRate(int sellingProductId, long saleZoneId, out CustomerZoneRate customerZoneRate)
        {
            var zoneRates = _reader.GetZoneRates(SalePriceListOwnerType.SellingProduct, sellingProductId);
            SaleRate saleRate;
            if (zoneRates != null && zoneRates.TryGetValue(saleZoneId, out saleRate))
            {
                customerZoneRate = new CustomerZoneRate
                {
                    Rate = saleRate,
                    Source = CustomerZoneRateSource.Product
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

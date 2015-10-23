using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using TOne.WhS.Sales.Entities.Queries;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class RatePlanManager
    {
        public IDataRetrievalResult<RatePlanItem> GetFilteredRatePlanItems(Vanrise.Entities.DataRetrievalInput<RatePlanQuery> input)
        {
            List<SaleZone> saleZones = GetSaleZones(input.Query.CustomerId, DateTime.Now);
            List<RatePlanItem> ratePlanItems = new List<RatePlanItem>();

            if (saleZones != null)
            {
                List<SaleRate> saleRates = GetSaleRatesByZoneIds(saleZones.Select(item => item.SaleZoneId).ToList()); // get the sale rates

                foreach (SaleZone zone in saleZones)
                {
                    RatePlanItem ratePlanItem = new RatePlanItem();

                    ratePlanItem.ZoneId = zone.SaleZoneId;
                    ratePlanItem.ZoneName = zone.Name;
                    ratePlanItem.Rate = saleRates.Where(item => item.ZoneId == zone.SaleZoneId).Single().NormalRate;

                    ratePlanItems.Add(ratePlanItem);
                }
            }

            BigResult<RatePlanItem> bigResult = new BigResult<RatePlanItem>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, bigResult);
        }

        private List<SaleZone> GetSaleZones(int customerId, DateTime? effectiveOn)
        {
            int saleZonePackageId = GetSaleZonePackageId(customerId);
            List<long> saleZoneIds = GetSaleZoneIds(customerId, effectiveOn);

            if (saleZoneIds == null) return null;
            
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            return saleZoneManager.GetSaleZonesByIds(saleZonePackageId, saleZoneIds).ToList();
        }

        private int GetSaleZonePackageId(int customerId)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            CarrierAccountDetail customer = manager.GetCarrierAccount(customerId);

            return customer.CustomerSettings.SaleZonePackageId;
        }

        private List<long> GetSaleZoneIds(int customerId, DateTime? effectiveOn)
        {
            CustomerZoneManager manager = new CustomerZoneManager();
            CustomerZones customerZone = manager.GetCustomerZone(customerId, effectiveOn, false);

            return (customerZone != null) ? customerZone.Zones.Select(item => item.ZoneId).ToList() : null;
        }

        private List<SaleRate> GetSaleRatesByZoneIds(List<long> zoneIds)
        {
            SaleRateManager manager = new SaleRateManager();
            return manager.GetSaleRatesByZoneIds(zoneIds);
        }
    }
}

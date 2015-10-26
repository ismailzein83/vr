using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using TOne.WhS.Sales.Entities.Queries;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class RatePlanManager
    {
        public IDataRetrievalResult<RatePlanItem> GetFilteredRatePlanItems(Vanrise.Entities.DataRetrievalInput<RatePlanQuery> input)
        {
            List<SaleZone> saleZones = GetSaleZones(input.Query.CustomerId);
            List<RatePlanItem> ratePlanItems = new List<RatePlanItem>();

            if (saleZones != null)
            {
                var saleZoneIds = saleZones.Select(item => item.SaleZoneId).ToList();
                List<SaleRate> saleRates = GetSaleRatesByCustomerZoneIds(input.Query.CustomerId, saleZoneIds);

                foreach (SaleZone saleZone in saleZones)
                {
                    RatePlanItem ratePlanItem = new RatePlanItem();

                    ratePlanItem.ZoneId = saleZone.SaleZoneId;
                    ratePlanItem.ZoneName = saleZone.Name;
                    var rate = saleRates.FindRecord(itm => itm.ZoneId == saleZone.SaleZoneId);
                    if (rate != null)
                        ratePlanItem.Rate = rate.NormalRate;

                    ratePlanItems.Add(ratePlanItem);
                }
            }

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, ratePlanItems.ToBigResult(input, null));
        }

        #region Private Methods

        private List<SaleZone> GetSaleZones(int customerId)
        {
            List<long> saleZoneIds = GetSaleZoneIds(customerId);
            if (saleZoneIds == null) return null;

            int sellingNumberPlanId = GetSellingNumberPlanId(customerId);

            SaleZoneManager saleZoneManager = new SaleZoneManager();
            return saleZoneManager.GetSaleZonesByIds(sellingNumberPlanId, saleZoneIds).ToList();
        }

        private int GetSellingNumberPlanId(int customerId)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            CarrierAccountDetail customer = manager.GetCarrierAccount(customerId);

            return customer.CustomerSettings.SellingNumberPlanId;
        }

        private List<long> GetSaleZoneIds(int customerId)
        {
            CustomerZoneManager manager = new CustomerZoneManager();
            CustomerZones customerZones = manager.GetCustomerZones(customerId, DateTime.Now, false);

            return (customerZones != null) ? customerZones.Zones.Select(x => x.ZoneId).ToList() : null;
        }

        private List<SaleRate> GetSaleRatesByCustomerZoneIds(int customerId, List<long> customerZoneIds)
        {
            SaleRateManager manager = new SaleRateManager();
            return manager.GetSaleRatesByCustomerZoneIds(customerId, customerZoneIds, DateTime.Now);
        }
        
        #endregion
    }
}

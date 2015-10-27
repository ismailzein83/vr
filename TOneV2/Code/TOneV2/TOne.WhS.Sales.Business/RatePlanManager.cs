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
        public List<RatePlanItem> GetRatePlanItems(RatePlanQuery query)
        {
            List<RatePlanItem> ratePlanItems = new List<RatePlanItem>();

            List<SaleZone> saleZones = GetSaleZones(query.CustomerId);
            
            if (saleZones != null)
            {
                saleZones =
                    saleZones.Where(x => x.Name == null || (x.Name.Length > 0 && char.ToLower(x.Name[0]) == char.ToLower(query.ZoneLetter))).ToList();

                List<long> saleZoneIds = saleZones.Select(item => item.SaleZoneId).ToList();
                List<SaleRate> saleRates = GetSaleRatesByCustomerZoneIds(query.CustomerId, saleZoneIds);

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

            return ratePlanItems;
        }

        #region Private Methods

        private List<SaleZone> GetSaleZones(int customerId)
        {
            List<int> countryIds = GetCountryIds(customerId);

            if (countryIds != null)
            {
                int sellingNumberPlanId = GetSellingNumberPlanId(customerId);

                SaleZoneManager saleZoneManager = new SaleZoneManager();
                return saleZoneManager.GetSaleZonesByCountryIds(sellingNumberPlanId, countryIds).ToList();
            }
            
            return null;
        }

        private List<int> GetCountryIds(int customerId)
        {
            CustomerZoneManager manager = new CustomerZoneManager();
            CustomerZones customerZones = manager.GetCustomerZones(customerId, DateTime.Now, false);

            return (customerZones != null && customerZones.Countries != null && customerZones.Countries.Count > 0) ?
                customerZones.Countries.Select(x => x.CountryId).ToList() : null;
        }

        private int GetSellingNumberPlanId(int customerId)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            CarrierAccountDetail customer = manager.GetCarrierAccount(customerId);

            return customer.CustomerSettings.SellingNumberPlanId;
        }

        private List<SaleRate> GetSaleRatesByCustomerZoneIds(int customerId, List<long> customerZoneIds)
        {
            SaleRateManager manager = new SaleRateManager();
            return manager.GetSaleRatesByCustomerZoneIds(customerId, customerZoneIds, DateTime.Now);
        }
        
        #endregion
    }
}

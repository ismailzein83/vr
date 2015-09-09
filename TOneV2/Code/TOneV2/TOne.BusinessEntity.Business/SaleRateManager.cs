using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;
using TOne.Caching;
using TOne.Entities;

namespace TOne.BusinessEntity.Business
{
    public class SaleRateManager
    {
        TOneCacheManager _cacheManager;
        public SaleRateManager(TOneCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public Rate GetSaleRates(string customerId, int zoneId, DateTime when)
        {
            CustomerRateByZone customerRateByZone = _cacheManager.GetOrCreateObject(String.Format("GetEffectiveSaleRates_{0}_{1:ddMMMyy}", customerId, when.Date),
           CacheObjectType.Pricing,
             () =>
             {
                 return GetCustomerRatesFromDB(customerId, when);
             });

            Rate rate;
            if (customerRateByZone.TryGetValue(zoneId, out rate))
                return rate;
            return null;
        }

        private static CustomerRateByZone GetCustomerRatesFromDB(string customerId, DateTime when)
        {
            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            List<Rate> rates = dataManager.GetSaleRates(customerId, when);
            CustomerRateByZone ratesDictionary = new CustomerRateByZone();
            if (rates != null && rates.Count > 0)
            {
                foreach (Rate rate in rates)
                {
                    if (!ratesDictionary.ContainsKey(rate.ZoneId))
                        ratesDictionary.Add(rate.ZoneId, rate);
                }
            }
            return ratesDictionary;
        }
    }
}

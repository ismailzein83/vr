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
    public class SupplierRateManager
    {
        TOneCacheManager _cacheManager;
        public SupplierRateManager(TOneCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }
        public Rate GetSupplierRates(string supplierId, int zoneId, DateTime when, bool IsRepricing)
        {
            SupplierRateByZone supplierRateByZone = _cacheManager.GetOrCreateObject(String.Format("GetEffectiveSupplierRates_{0}_{1:ddMMMyy}", supplierId, when.Date),
            CacheObjectType.Pricing,
              () =>
              {
                  return GetSupplierRatesFromDB(supplierId, when);
              });

            Rate rate;
            if (supplierRateByZone.TryGetValue(zoneId, out rate))
                return rate;
            return null;
        }
        private static SupplierRateByZone GetSupplierRatesFromDB(string supplierId, DateTime when)
        {
            ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
            List<Rate> rates = dataManager.GetSupplierRates(supplierId, when);
            SupplierRateByZone ratesDictionary = new SupplierRateByZone();
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

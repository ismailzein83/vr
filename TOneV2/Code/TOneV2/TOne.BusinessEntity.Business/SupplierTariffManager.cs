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
    public class SupplierTariffManager
    {
        TOneCacheManager _cacheManager;
        public SupplierTariffManager()
        {
        }
        public SupplierTariffManager(TOneCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }
        public Vanrise.Entities.IDataRetrievalResult<SupplierTariff> GetFilteredSupplierTariffs(Vanrise.Entities.DataRetrievalInput<SupplierTariffQuery> input)
        {
            ISupplierTariffDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierTariffDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredSupplierTariffs(input));
        }
        public SupplierTariff GetSupplierTariff(string supplierId, int zoneId, DateTime when)
        {

            TariffsBySupplier tariffsBySupplier = _cacheManager.GetOrCreateObject(String.Format("GetEffectiveSupplierTariff_{0}_{1:ddMMMyy}", supplierId, when.Date),
              CacheObjectType.Pricing,
              () =>
              {
                  return GetSupplierTariffsFromDB(when);
              });

            SupplierTariffsByZone tariffByZoneObject;
            if (tariffsBySupplier.TryGetValue(supplierId, out tariffByZoneObject))
            {
                SupplierTariff tariff;
                tariffByZoneObject.TryGetValue(zoneId, out tariff);
                return tariff;
            }
            return null;
        }

        private static TariffsBySupplier GetSupplierTariffsFromDB(DateTime when)
        {
            ISupplierTariffDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierTariffDataManager>();
            List<SupplierTariff> supplierTariffs = dataManager.GetSupplierTariffs(when);
            TariffsBySupplier tariffsBySupplier = new TariffsBySupplier();
            foreach (SupplierTariff tariff in supplierTariffs)
            {
                SupplierTariffsByZone supplierTariffsByZone;
                if (!tariffsBySupplier.TryGetValue(tariff.SupplierID, out supplierTariffsByZone))
                {
                    supplierTariffsByZone = new SupplierTariffsByZone();
                    tariffsBySupplier.Add(tariff.SupplierID, supplierTariffsByZone);
                }
                if (!supplierTariffsByZone.ContainsKey(tariff.ZoneID))
                    supplierTariffsByZone.Add(tariff.ZoneID, tariff);
            }
            return tariffsBySupplier;
        }
    }
}

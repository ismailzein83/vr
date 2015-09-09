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
    public class SupplierCommissionManager
    {
         TOneCacheManager _cacheManager;
         public SupplierCommissionManager()
         {
         }
         public SupplierCommissionManager(TOneCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }
         public Vanrise.Entities.IDataRetrievalResult<SupplierCommission> GetSupplierCommissions(Vanrise.Entities.DataRetrievalInput<SupplierCommissionQuery> input)
        {
            ISupplierCommissionDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierCommissionDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetSupplierCommissions(input));
        }
        public SupplierCommission GetSupplierCommissions(string supplierId, int zoneId, DateTime when)
        {
            CommissionsBySupplier commissionsBySupplier = _cacheManager.GetOrCreateObject(String.Format("GetEffectiveSupplierCommission_{0}_{1:ddMMMyy}", supplierId, when.Date),
              CacheObjectType.Pricing,
              () =>
              {
                  return GetSupplierCommissionsFromDB(when);
              });

            SupplierCommissionsByZone supplierCommissionsByZone;
            if (commissionsBySupplier.TryGetValue(supplierId, out supplierCommissionsByZone))
            {
                SupplierCommission commission;
                supplierCommissionsByZone.TryGetValue(zoneId, out commission);
                return commission;
            }
            return null;
        }

        private CommissionsBySupplier GetSupplierCommissionsFromDB(DateTime when)
        {
            ISupplierCommissionDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierCommissionDataManager>();
            List<SupplierCommission> supplierCommissions = dataManager.GetSupplierCommissions(when);
            CommissionsBySupplier commissionsBySupplier = new CommissionsBySupplier();
            foreach (SupplierCommission commission in supplierCommissions)
            {
                SupplierCommissionsByZone supplierCommissionsByZone;
                if (!commissionsBySupplier.TryGetValue(commission.CustomerId, out supplierCommissionsByZone))
                {
                    supplierCommissionsByZone = new SupplierCommissionsByZone();
                    commissionsBySupplier.Add(commission.CustomerId, supplierCommissionsByZone);
                }
                if (!supplierCommissionsByZone.ContainsKey(commission.ZoneId))
                    supplierCommissionsByZone.Add(commission.ZoneId, commission);
            }
            return commissionsBySupplier;
        }
    }
}

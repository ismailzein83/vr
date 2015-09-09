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

    public class SupplierTODManager : BaseTODManager<SupplierTODConsiderationInfo>
    {
           TOneCacheManager _cacheManager;
         public SupplierTODManager()
         {
         }
         public SupplierTODManager(TOneCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }
        public Vanrise.Entities.IDataRetrievalResult<SupplierTODConsiderationInfo> GetSupplierTODFiltered(Vanrise.Entities.DataRetrievalInput<TODQuery> input)
        {
            AccountManagerManager am = new AccountManagerManager();
            List<string> suppliersAMUIds = am.GetMyAssignedSupplierIds();
            ISupplierTODDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierTODDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetSupplierToDConsiderationByCriteria(input, suppliersAMUIds));
        }
       public SupplierTODConsiderationInfo GetSuppliersToDConsideration(string supplierId, int zoneId, DateTime when)
       {
           TODConsiderationsBySupplier todConsiderationsBySupplier = _cacheManager.GetOrCreateObject(String.Format("GetEffectiveSupplierTODConsideration_{0}_{1:ddMMMyy}", supplierId, when.Date),
           CacheObjectType.Pricing,
           () =>
           {
               return GetSupplierToDConsiderationsFromDB(when);
           });

           SupplierToDConsiderationByZone toDConsiderationByZoneObject;
           if (todConsiderationsBySupplier.TryGetValue(supplierId, out toDConsiderationByZoneObject))
           {
               SupplierTODConsiderationInfo todConsiderationInfo;
               toDConsiderationByZoneObject.TryGetValue(zoneId, out todConsiderationInfo);
               return todConsiderationInfo;
           }
           return null;
       }
       private static TODConsiderationsBySupplier GetSupplierToDConsiderationsFromDB(DateTime when)
       {
           ISupplierTODDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierTODDataManager>();
           List<SupplierTODConsiderationInfo> suppliersTODConsiderationInfo = dataManager.GetSuppliersToDConsideration(when);
           TODConsiderationsBySupplier todConsiderationsBySupplier = new TODConsiderationsBySupplier();
           foreach (SupplierTODConsiderationInfo todConsiderationInfo in suppliersTODConsiderationInfo)
           {
               SupplierToDConsiderationByZone toDConsiderationByZoneObject;
               if (!todConsiderationsBySupplier.TryGetValue(todConsiderationInfo.SupplierID, out toDConsiderationByZoneObject))
               {
                   toDConsiderationByZoneObject = new SupplierToDConsiderationByZone();
                   todConsiderationsBySupplier.Add(todConsiderationInfo.SupplierID, toDConsiderationByZoneObject);
               }
               if (!toDConsiderationByZoneObject.ContainsKey(todConsiderationInfo.ZoneID))
                   toDConsiderationByZoneObject.Add(todConsiderationInfo.ZoneID, todConsiderationInfo);
           }
           return todConsiderationsBySupplier;
       }
    }
}

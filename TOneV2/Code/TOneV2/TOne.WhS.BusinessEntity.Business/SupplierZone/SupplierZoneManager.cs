using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierZoneManager
    {

        public IEnumerable<SupplierZoneInfo> GetSupplierZones(int supplierId)
        {
            List<SupplierZone> supplierZones = GetCachedSupplierZones();
            return supplierZones.MapRecords(SupplierZoneInfoMapper,x => x.SupplierId == supplierId);
           
        }

        public List<SupplierZone> GetSupplierZones(int supplierId, DateTime effectiveDate)
        {
            ISupplierZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
            return dataManager.GetSupplierZones(supplierId, effectiveDate);
        }
        public SupplierZone GetSupplierZone(long zoneId)
        {
            throw new NotImplementedException();
        }


        public long ReserveIDRange(int numberOfIDs)
        {
            ISupplierZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
            return dataManager.ReserveIDRange(numberOfIDs);
        }
        #region Private Members

        List<SupplierZone> GetCachedSupplierZones()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSupplierZones",
               () =>
               {
                   ISupplierZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
                   return dataManager.GetSupplierZones(DateTime.Now);
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISupplierZoneDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSupplierZonesUpdated(ref _updateHandle);
            }
        }

        private SupplierZoneInfo SupplierZoneInfoMapper(SupplierZone supplierZone)
        {
            return new SupplierZoneInfo()
            {
                SupplierZoneId = supplierZone.SupplierZoneId,
                Name = supplierZone.Name,
            };
        }

        #endregion

    }
}

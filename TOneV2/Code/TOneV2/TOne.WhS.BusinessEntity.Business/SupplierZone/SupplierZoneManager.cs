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

        public IEnumerable<SupplierZoneInfo> GetSupplierZoneInfo(SupplierZoneInfoFilter filter, string searchValue)
        {
            IEnumerable<SupplierZone> supplierZones = null;
            if(filter!=null)
                supplierZones = GetSupplierZoneBySupplier(filter);
            else
             supplierZones = GetCachedSupplierZones();
            return supplierZones.MapRecords(SupplierZoneInfoMapper, x => x.Name.Contains(searchValue));
           
        }
        private IEnumerable<SupplierZone> GetSupplierZoneBySupplier(SupplierZoneInfoFilter filter)
        {
            IEnumerable<SupplierZone> supplierZones = GetCachedSupplierZones();
            Func<SupplierZone, bool> filterExpression = (item) =>
                 (item.SupplierId == filter.SupplierId);

            return supplierZones.FindAllRecords(filterExpression);
        }
       
        public List<SupplierZone> GetSupplierZones(int supplierId, DateTime effectiveDate)
        {
            ISupplierZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
            return dataManager.GetSupplierZones(supplierId, effectiveDate);
        }
        public SupplierZone GetSupplierZone(long zoneId)
        {
            List<SupplierZone> supplierZones = GetCachedSupplierZones();
            return supplierZones.FindRecord(x => x.SupplierZoneId == zoneId);
        }
        public IEnumerable<SupplierZoneInfo> GetSupplierZoneInfoByIds(List<long> selectedIds)
        {
            List<SupplierZone> allSupplierZones = GetCachedSupplierZones();
            return allSupplierZones.MapRecords(SupplierZoneInfoMapper, x => selectedIds.Contains(x.SupplierZoneId));
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

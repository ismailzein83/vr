using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierZoneManager
    {
        public Vanrise.Entities.IDataRetrievalResult<SupplierZoneDetails> GetFilteredSupplierZones(Vanrise.Entities.DataRetrievalInput<SupplierZoneQuery> input)
        {
            var allsupplierZones = GetCachedSupplierZones();
            Func<SupplierZone, bool> filterExpression = (prod) =>
                     (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    && (input.Query.Countries == null || input.Query.Countries.Contains(prod.CountryId))
                  && (input.Query.SupplierId.Equals(prod.SupplierId))
                  && ((!input.Query.EffectiveOn.HasValue || (prod.BED <= input.Query.EffectiveOn)))
                  && ((!input.Query.EffectiveOn.HasValue || !prod.EED.HasValue || (prod.EED > input.Query.EffectiveOn)));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allsupplierZones.ToBigResult(input, filterExpression, SupplierZoneDetailMapper));
        }
        
        public IEnumerable<SupplierZoneInfo> GetSupplierZoneInfo(SupplierZoneInfoFilter filter, string searchValue)
        {
            IEnumerable<SupplierZone> supplierZones = null;
            if(filter!=null)
                supplierZones = GetSupplierZoneBySupplier(filter);
            else
             supplierZones = GetCachedSupplierZones();
            return supplierZones.MapRecords(SupplierZoneInfoMapper, x => x.Name.ToLower().Contains(searchValue.ToLower()));
           
        }
        
        public List<SupplierZone> GetSupplierZonesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            ISupplierZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
            return dataManager.GetSupplierZonesEffectiveAfter(supplierId, minimumDate);
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
            long startingId;
            IDManager.Instance.ReserveIDRange(this.GetType(), numberOfIDs, out startingId);
            return startingId;
        }

        #region Private Members

        List<SupplierZone> GetCachedSupplierZones()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSupplierZones",
               () =>
               {
                   ISupplierZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
                   return dataManager.GetSupplierZones();
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

        private SupplierZoneDetails SupplierZoneDetailMapper(SupplierZone supplierZone)
        {
            SupplierZoneDetails supplierZoneDetail = new SupplierZoneDetails();

            supplierZoneDetail.Entity = supplierZone;

            CountryManager countryManager = new CountryManager();
            CarrierAccountManager caManager = new CarrierAccountManager();
            var country = countryManager.GetCountry(supplierZone.CountryId);
            if (country != null)
                supplierZoneDetail.CountryName = country.Name;
            
            int supplierId = supplierZone.SupplierId;
            supplierZoneDetail.SupplierName = caManager.GetCarrierAccount(supplierId).Name;
            return supplierZoneDetail;
        }

        private IEnumerable<SupplierZone> GetSupplierZoneBySupplier(SupplierZoneInfoFilter filter)
        {
            IEnumerable<SupplierZone> supplierZones = GetCachedSupplierZones();
            Func<SupplierZone, bool> filterExpression = (item) =>
                 (item.SupplierId == filter.SupplierId);

            return supplierZones.FindAllRecords(filterExpression);
        }
        #endregion

    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierZoneManager : IBusinessEntityManager
    {
        #region Public Methods
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
            if (filter != null)
                supplierZones = GetSupplierZoneBySupplier(filter);
            else
                supplierZones = GetCachedSupplierZones().Values;
            return supplierZones.MapRecords(SupplierZoneInfoMapper, x => x.Name.ToLower().Contains(searchValue.ToLower())).OrderBy(x => x.Name);
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
            var supplierZones = GetCachedSupplierZones();
            return supplierZones.GetRecord(zoneId);
        }
        public string GetSupplierZoneName(long zoneId)
        {
            SupplierZone supplierZone = GetSupplierZone(zoneId);
            return supplierZone != null ? supplierZone.Name : null;
        }
        public IEnumerable<SupplierZoneInfo> GetSupplierZoneInfoByIds(List<long> selectedIds)
        {
            var allSupplierZones = GetCachedSupplierZones();
            return allSupplierZones.MapRecords(SupplierZoneInfoMapper, x => selectedIds.Contains(x.SupplierZoneId)).OrderBy(x => x.Name);
        }
        public long ReserveIDRange(int numberOfIDs)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(this.GetType(), numberOfIDs, out startingId);
            return startingId;
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetSupplierZoneName((long)context.EntityId);
        }

        public string GetDescription(IEnumerable<long> supplierZoneIds)
        {
            IEnumerable<SupplierZone> supplierZones = GetCachedSupplierZones().Values;
            Func<SupplierZone, bool> filterExpression = null;
            if (supplierZoneIds != null)
                filterExpression = (itm) => (supplierZoneIds.Contains(itm.SupplierZoneId));
            var supplierZoneIdsValues = supplierZones.FindAllRecords(filterExpression);
            if (supplierZoneIdsValues != null)
                return string.Join(", ", supplierZoneIdsValues.Select(x => x.Name));
            return string.Empty;
        }

        public IEnumerable<SupplierZoneGroupTemplate> GetSupplierZoneGroupTemplates()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<SupplierZoneGroupTemplate>(Constants.SupplierZoneGroupTemplate);
        }

        #endregion

        #region Private Members

        Dictionary<long, SupplierZone> GetCachedSupplierZones()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSupplierZones",
               () =>
               {
                   ISupplierZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
                   return dataManager.GetSupplierZones().ToDictionary(itm => itm.SupplierZoneId, itm => itm);
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISupplierZoneDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
            object _updateHandle;

            public override Vanrise.Caching.CacheObjectSize ApproximateObjectSize
            {
                get
                {
                    return Vanrise.Caching.CacheObjectSize.ExtraLarge;
                }
            }

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
            supplierZoneDetail.SupplierName = caManager.GetCarrierAccountName(supplierId);
            return supplierZoneDetail;
        }

        private IEnumerable<SupplierZone> GetSupplierZoneBySupplier(SupplierZoneInfoFilter filter)
        {
            var supplierZones = GetCachedSupplierZones();
            Func<SupplierZone, bool> filterExpression = (item) =>
                 (item.SupplierId == filter.SupplierId);

            return supplierZones.FindAllRecords(filterExpression);
        }
        #endregion

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetSupplierZone(context.EntityId);
        }

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var allZones = GetCachedSupplierZones();
            if (allZones == null)
                return null;
            else
                return allZones.Values.Select(itm => itm as dynamic).ToList();
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }
    }
}

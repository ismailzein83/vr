using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Caching;
using Vanrise.Caching.Runtime;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierZoneManager : IBusinessEntityManager, ISupplierZoneManager
    {
        #region Public Methods

        public Dictionary<long, SupplierZone> GetCachedSupplierZones()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSupplierZones",
               () =>
               {
                   DistributedCacher cacher = new DistributedCacher();
                   Func<SupplierZoneCachedObjectCreationHandler> objectCreationHandler = () => { return new SupplierZoneCachedObjectCreationHandler(); };
                   return cacher.GetOrCreateObject<CacheManager, Dictionary<long, SupplierZone>>("Distributed_GetSupplierZones", objectCreationHandler);
               });
        }

        public List<long> GetSupplierZoneIdsByDates(int supplierId, DateTime fromDate, DateTime? toDate)
        {
            List<long> matchingSupplierZoneIds = new List<long>();
            var allsupplierZones = GetCachedSupplierZones();
            Func<SupplierZone, bool> filterExpression = (item) =>
                {
                    if (item.SupplierId != supplierId)
                        return false;

                    if (toDate.HasValue && toDate < item.BED)
                        return false;

                    if (item.EED.HasValue && fromDate > item.EED)
                        return false;

                    return true;

                };
            var matchingSupplierZones = allsupplierZones.FindAllRecords(filterExpression);
            if (matchingSupplierZones != null)
                matchingSupplierZoneIds = matchingSupplierZones.Select(itm => itm.SupplierZoneId).ToList();

            return matchingSupplierZoneIds;
        }

        public Vanrise.Entities.IDataRetrievalResult<SupplierZoneDetails> GetFilteredSupplierZones(Vanrise.Entities.DataRetrievalInput<SupplierZoneQuery> input)
        {
            var allsupplierZones = GetCachedSupplierZones();
            Func<SupplierZone, bool> filterExpression = (prod) =>
                     (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    && (input.Query.Countries == null || input.Query.Countries.Contains(prod.CountryId))
                  && (input.Query.SupplierId.Equals(prod.SupplierId))
                  && ((prod.BED <= input.Query.EffectiveOn))
                  && ((!prod.EED.HasValue || (prod.EED > input.Query.EffectiveOn)));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allsupplierZones.ToBigResult(input, filterExpression, SupplierZoneDetailMapper));
        }
        public IEnumerable<SupplierZoneInfo> GetSupplierZoneInfo(SupplierZoneInfoFilter filter, int supplierId, string searchValue)
        {
            string nameFilterLower = searchValue != null ? searchValue.ToLower() : null;
            DateTime now = DateTime.Now;

            IEnumerable<SupplierZone> supplierZones = GetSupplierZonesBySupplier(supplierId);

            Func<SupplierZone, bool> filterExpression = (supplierZone) =>
            {
				if (filter.CountryIds != null && !filter.CountryIds.Contains(supplierZone.CountryId))
					return false;

                if (filter.GetEffectiveOnly && (supplierZone.BED > now || (supplierZone.EED.HasValue && supplierZone.EED.Value < now)))
                    return false;

                if (nameFilterLower != null && !supplierZone.Name.ToLower().Contains(nameFilterLower))
                    return false;

                if (filter.AvailableZoneIds != null && !filter.AvailableZoneIds.Contains(supplierZone.SupplierZoneId))
                    return false;

                if (filter.ExcludedZoneIds != null && filter.ExcludedZoneIds.Contains(supplierZone.SupplierZoneId))
                    return false;

                return true;
            };

            return supplierZones.MapRecords(SupplierZoneInfoMapper, filterExpression).OrderBy(item => item.Name);
        }

        public IEnumerable<SupplierZoneInfo> GetSupplierZonesInfo(int supplierId)
        {
            IEnumerable<SupplierZone> supplierZones = GetCachedSupplierZones().Values;
            supplierZones = supplierZones.FindAllRecords(item => item.SupplierId == supplierId).OrderBy(item => item.Name);

            return supplierZones.MapRecords(SupplierZoneInfoMapper);
        }

        public IEnumerable<int> GetDistinctSupplierIdsBySupplierZoneIds(IEnumerable<long> supplierZoneIds)
        {
            return this.GetCachedSupplierZones().MapRecords(zone => zone.SupplierId, zone => supplierZoneIds.Contains(zone.SupplierZoneId)).Distinct();
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
            IDManager.Instance.ReserveIDRange(GetSupplierZoneType(), numberOfIDs, out startingId);
            return startingId;
        }

        public int GetSupplierZoneTypeId()
        {
            return Vanrise.Common.Business.TypeManager.Instance.GetTypeId(this.GetSupplierZoneType());
        }

        public Type GetSupplierZoneType()
        {
            return this.GetType();
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetSupplierZoneName(Convert.ToInt64(context.EntityId));
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
            return templateConfigManager.GetExtensionConfigurations<SupplierZoneGroupTemplate>(SupplierZoneGroupTemplate.EXTENSION_TYPE);
        }

        #endregion

        #region Private Members

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

        private class SupplierZoneCachedObjectCreationHandler : CachedObjectCreationHandler<Dictionary<long, SupplierZone>>
        {
            public override Dictionary<long, SupplierZone> CreateObject()
            {
                ISupplierZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
                List<SupplierZone> allSupplierZones = dataManager.GetSupplierZones();
                if (allSupplierZones == null)
                    return null;

                return allSupplierZones.ToDictionary(itm => itm.SupplierZoneId, itm => itm);
            }
        }

        #endregion


        #region Private Methods

        private IEnumerable<SupplierZone> GetSupplierZonesBySupplier(int supplierId)
        {
            IEnumerable<SupplierZone> supplierZones = GetCachedSupplierZones().Values;
            return supplierZones.FindAllRecords(item => item.SupplierId == supplierId);
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


        public IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            Func<SupplierZone, bool> filter;
            switch (context.ParentEntityDefinition.Name)
            {
                case Vanrise.Entities.Country.BUSINESSENTITY_DEFINITION_NAME: filter = (zone) => zone.CountryId == context.ParentEntityId; break;
                default: throw new NotImplementedException(String.Format("Business Entity Definition Name '{0}'", context.ParentEntityDefinition.Name));
            }
            return GetCachedSupplierZones().FindAllRecords(filter).MapRecords(zone => zone.SupplierZoneId as dynamic);
        }

        public dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            var supplierZone = context.Entity as SupplierZone;
            if (supplierZone == null)
                throw new NullReferenceException("supplierZone");
            switch (context.ParentEntityDefinition.Name)
            {
                case Vanrise.Entities.Country.BUSINESSENTITY_DEFINITION_NAME: return supplierZone.CountryId;
                default: throw new NotImplementedException(String.Format("Business Entity Definition Name '{0}'", context.ParentEntityDefinition.Name));
            }
        }


        public dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }
    }
}

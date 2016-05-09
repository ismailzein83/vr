using System;
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
    public class SaleZoneManager : IBusinessEntityManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<SaleZoneDetail> GetFilteredSaleZones(Vanrise.Entities.DataRetrievalInput<SaleZoneQuery> input)
        {
            var saleZonesBySellingNumberPlan = GetSaleZonesBySellingNumberPlan(input.Query.SellingNumberId);
            Func<SaleZone, bool> filterExpression = (prod) =>
                     (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    && (input.Query.Countries == null || input.Query.Countries.Contains(prod.CountryId))
                  && ((!input.Query.EffectiveOn.HasValue || (prod.BED <= input.Query.EffectiveOn)))
                  && ((!input.Query.EffectiveOn.HasValue || !prod.EED.HasValue || (prod.EED > input.Query.EffectiveOn)));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, saleZonesBySellingNumberPlan.ToBigResult(input, filterExpression, SaleZoneDetailMapper));
        }

        public IEnumerable<SaleZone> GetSaleZones(int sellingNumberPlanId, DateTime effectiveDate)
        {
            return GetSaleZonesBySellingNumberPlan(sellingNumberPlanId).FindAllRecords(item => item.IsEffective(effectiveDate));
        }

        public IEnumerable<SaleZone> GetSaleZonesBySellingNumberPlan(int sellingNumberPlanId)
        {
            IEnumerable<SaleZone> allSaleZones = GetCachedSaleZones().Values;
            return allSaleZones.FindAllRecords(x => x.SellingNumberPlanId == sellingNumberPlanId);
        }

        public SaleZone GetSaleZone(long saleZoneId)
        {
            return GetCachedSaleZones().GetRecord(saleZoneId);
        }

        public IEnumerable<long> GetSaleZoneIds(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            ISaleZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleZoneDataManager>();
            return dataManager.GetSaleZoneIds(effectiveOn, isEffectiveInFuture);
        }

        public string GetDescription(int sellingNumberPlanId, IEnumerable<long> saleZoneIds)
        {
            IEnumerable<SaleZone> saleZonesBySellingNumberPlan = GetSaleZonesBySellingNumberPlan(sellingNumberPlanId);

            Func<SaleZone, bool> filterExpression = null;

            if (saleZoneIds != null)
                filterExpression = (itm) => (saleZoneIds.Contains(itm.SaleZoneId));

            saleZonesBySellingNumberPlan = saleZonesBySellingNumberPlan.FindAllRecords(filterExpression);

            if (saleZonesBySellingNumberPlan != null)
                return string.Join(", ", saleZonesBySellingNumberPlan.Select(x => x.Name));

            return string.Empty;
        }

        public List<Vanrise.Entities.TemplateConfig> GetSaleZoneGroupTemplates()
        {

            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.SaleZoneGroupConfigType);
        }

        public IEnumerable<SaleZone> GetSaleZonesByCountryIds(int sellingNumberPlanId, IEnumerable<int> countryIds)
        {
            IEnumerable<SaleZone> saleZonesBySellingNumberPlan = GetSaleZonesBySellingNumberPlan(sellingNumberPlanId);

            if (saleZonesBySellingNumberPlan != null)
                saleZonesBySellingNumberPlan = saleZonesBySellingNumberPlan.FindAllRecords(z => countryIds.Contains((int)z.CountryId));

            return saleZonesBySellingNumberPlan;
        }

        public IEnumerable<SaleZone> GetSaleZonesByCountryId(int sellingNumberPlanId, int countryId)
        {
            IEnumerable<SaleZone> saleZonesBySellingNumberPlan = GetSaleZonesBySellingNumberPlan(sellingNumberPlanId);

            if (saleZonesBySellingNumberPlan != null)
                saleZonesBySellingNumberPlan = saleZonesBySellingNumberPlan.FindAllRecords(z => (!z.EED.HasValue || (z.EED > DateTime.Now)) && countryId == z.CountryId);

            return saleZonesBySellingNumberPlan;
        }

        public IEnumerable<SaleZoneInfo> GetSaleZonesInfo(string nameFilter, int sellingNumberPlanId, SaleZoneInfoFilter filter)
        {
            string nameFilterLower = nameFilter != null ? nameFilter.ToLower() : null;
            IEnumerable<SaleZone> saleZonesBySellingNumberPlan = GetSaleZonesBySellingNumberPlan(sellingNumberPlanId);
            HashSet<long> filteredZoneIds = SaleZoneGroupContext.GetFilteredZoneIds(filter.SaleZoneFilterSettings);
            Func<SaleZone, bool> zoneFilter = (zone) =>
            {
                if (filteredZoneIds != null && !filteredZoneIds.Contains(zone.SaleZoneId))
                    return false;
                if (nameFilterLower != null && !zone.Name.ToLower().Contains(nameFilterLower))
                    return false;
                return true;
            };
            return saleZonesBySellingNumberPlan.MapRecords(SaleZoneInfoMapper, zoneFilter);
        }

        public IEnumerable<SaleZoneInfo> GetSaleZonesInfoByIds(HashSet<long> saleZoneIds, SaleZoneFilterSettings saleZoneFilterSettings)
        {
            IEnumerable<SaleZone> saleZonesBySellingNumberPlan = GetCachedSaleZones().Values;
            HashSet<long> filteredZoneIds = SaleZoneGroupContext.GetFilteredZoneIds(saleZoneFilterSettings);
            Func<SaleZone, bool> zoneFilter = (zone) =>
            {
                if (!saleZoneIds.Contains(zone.SaleZoneId))
                    return false;
                if (filteredZoneIds != null && !filteredZoneIds.Contains(zone.SaleZoneId))
                    return false;
                return true;
            };
            return saleZonesBySellingNumberPlan.MapRecords(SaleZoneInfoMapper, zoneFilter);
        }

        public IEnumerable<SaleZoneInfo> GetSellingNumberPlanIdBySaleZoneIds(List<long> saleZoneIds)
        {
            IEnumerable<SaleZone> saleZones = this.GetCachedSaleZones().Values;
            Func<SaleZone, bool> saleZoneFilter = (salezone) =>
            {
                if (!saleZoneIds.Contains(salezone.SaleZoneId))
                    return false;
                return true;
            };
            return saleZones.MapRecords(SaleZoneInfoMapper, saleZoneFilter);
        }

        public string GetSaleZoneName(long saleZoneId)
        {
            SaleZone saleZone = GetSaleZone(saleZoneId);

            if (saleZone != null)
                return saleZone.Name;

            return null;
        }

        public bool IsCountryHasSaleZones(int sellingNumberPlanId, int countryId, DateTime minimumDate)
        {
            return this.GetSaleZonesEffectiveAfter(sellingNumberPlanId, countryId, minimumDate).Count() > 0;
        }

        public IEnumerable<SaleZone> GetSaleZonesEffectiveAfter(int sellingNumberPlanId, int countryId, DateTime minimumDate)
        {
            IEnumerable<SaleZone> allSaleZones = GetCachedSaleZones().Values;
            return allSaleZones.FindAllRecords(item => item.SellingNumberPlanId == sellingNumberPlanId && item.CountryId == countryId && (!item.EED.HasValue || (item.EED.Value > minimumDate && item.EED > item.BED)));
        }
        public IEnumerable<SaleZone> GetEffectiveSaleZonesBySellingNumberPlan(int sellingNumberPlanId, DateTime minimumDate)
        {
            IEnumerable<SaleZone> allSaleZones = GetCachedSaleZones().Values;
            return allSaleZones.FindAllRecords(item => item.SellingNumberPlanId == sellingNumberPlanId && (!item.EED.HasValue || (item.EED.Value > DateTime.Now && item.EED > item.BED)));
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            var saleZoneNames = new List<string>();
            foreach (var entityId in context.EntityIds)
            {
                string saleZoneName = GetSaleZoneName(Convert.ToInt64(entityId));
                if (saleZoneName == null) throw new NullReferenceException("saleZoneName");
                saleZoneNames.Add(saleZoneName);
            }
            return String.Join(",", saleZoneNames);
        }

        public bool IsMatched(IBusinessEntityMatchContext context)
        {
            if (context.FieldValueIds == null || context.FilterIds == null) return true;

            var fieldValueIds = context.FieldValueIds.MapRecords(itm => Convert.ToInt64(itm));
            var filterIds = context.FilterIds.MapRecords(itm => Convert.ToInt64(itm));
            foreach (var filterId in filterIds)
            {
                if (fieldValueIds.Contains(filterId))
                    return true;
            }
            return false;
        }

        #endregion
        
        #region Private Members

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISaleZoneDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISaleZoneDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return _dataManager.AreZonesUpdated(ref _updateHandle);
            }
        }

        private Dictionary<long, SaleZone> GetCachedSaleZones()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllSaleZones", () =>
            {
                ISaleZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleZoneDataManager>();
                IEnumerable<SaleZone> allSaleZones = dataManager.GetAllSaleZones();
                Dictionary<long, SaleZone> allSaleZonesDic = new Dictionary<long, SaleZone>();
                if (allSaleZones != null)
                {
                    foreach (var saleZone in allSaleZones)
                    {
                        allSaleZonesDic.Add(saleZone.SaleZoneId, saleZone);
                    }
                }
                return allSaleZonesDic;
            });
        }

        private SaleZoneDetail SaleZoneDetailMapper(SaleZone saleZone)
        {
            SaleZoneDetail saleZoneDetail = new SaleZoneDetail();

            saleZoneDetail.Entity = saleZone;

            CountryManager manager = new CountryManager();
            SellingNumberPlanManager sellingManager = new SellingNumberPlanManager();

            int countryId = saleZone.CountryId;
            var country = manager.GetCountry(countryId);
            saleZoneDetail.CountryName = (country != null) ? country.Name : "";

            int sellingNumberPlanId = saleZone.SellingNumberPlanId;
            var sellingNumberPlan = sellingManager.GetSellingNumberPlan(sellingNumberPlanId);
            saleZoneDetail.SellingNumberPlanName = (sellingNumberPlan != null) ? sellingNumberPlan.Name : "";


            return saleZoneDetail;
        }
        public long ReserveIDRange(int numberOfIDs)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(this.GetType(), numberOfIDs, out startingId);
            return startingId;
        }
        private SaleZoneInfo SaleZoneInfoMapper(SaleZone saleZone)
        {
            return new SaleZoneInfo { SaleZoneId = saleZone.SaleZoneId, Name = saleZone.Name , SellingNumberPlanId =  saleZone.SellingNumberPlanId};
        }

        #endregion
    }
}

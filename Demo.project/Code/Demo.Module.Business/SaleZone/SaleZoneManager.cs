using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Data;
using Demo.Module.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class SaleZoneManager
    {

        public Vanrise.Entities.IDataRetrievalResult<SaleZoneDetail> GetFilteredSaleZones(Vanrise.Entities.DataRetrievalInput<SaleZoneQuery> input)
        {
            var saleZones = GetSaleZones();
            Func<SaleZone, bool> filterExpression = (prod) =>
                     (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    && (input.Query.Countries == null || input.Query.Countries.Contains(prod.CountryId))
                  && ((!input.Query.EffectiveOn.HasValue || (prod.BED <= input.Query.EffectiveOn)))
                  && ((!input.Query.EffectiveOn.HasValue || !prod.EED.HasValue || (prod.EED > input.Query.EffectiveOn)));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, saleZones.ToBigResult(input, filterExpression, SaleZoneDetailMapper));
        }
        
        public IEnumerable<SaleZone> GetSaleZones( DateTime effectiveDate)
        {
            return GetSaleZones().FindAllRecords(item => item.IsEffective(effectiveDate));
        }

        public IEnumerable<SaleZone> GetSaleZones()
        {
            IEnumerable<SaleZone> allSaleZones = GetCachedSaleZones().Values;
            return allSaleZones;
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

        public string GetDescription( IEnumerable<long> saleZoneIds)
        {
            IEnumerable<SaleZone> saleZonesBySellingNumberPlan = GetSaleZones();

            Func<SaleZone, bool> filterExpression = null;

            if (saleZoneIds != null)
                filterExpression = (itm) => (saleZoneIds.Contains(itm.SaleZoneId));

            saleZonesBySellingNumberPlan = saleZonesBySellingNumberPlan.FindAllRecords(filterExpression);

            if (saleZonesBySellingNumberPlan != null)
                return string.Join(", ", saleZonesBySellingNumberPlan.Select(x => x.Name));

            return string.Empty;
        }

        //public List<Vanrise.Entities.TemplateConfig> GetSaleZoneGroupTemplates()
        //{

        //    TemplateConfigManager manager = new TemplateConfigManager();
        //    return manager.GetTemplateConfigurations(Constants.SaleZoneGroupConfigType);
        //}

        public IEnumerable<SaleZone> GetSaleZonesByCountryIds( IEnumerable<int> countryIds)
        {
            IEnumerable<SaleZone> saleZonesBySellingNumberPlan = GetSaleZones();

            if (saleZonesBySellingNumberPlan != null)
                saleZonesBySellingNumberPlan = saleZonesBySellingNumberPlan.FindAllRecords(z => countryIds.Contains((int)z.CountryId));

            return saleZonesBySellingNumberPlan;
        }

        public IEnumerable<SaleZone> GetSaleZonesByCountryId(int countryId)
        {
            IEnumerable<SaleZone> saleZonesBySellingNumberPlan = GetSaleZones();

            if (saleZonesBySellingNumberPlan != null)
                saleZonesBySellingNumberPlan = saleZonesBySellingNumberPlan.FindAllRecords(z => z.BED <= DateTime.Now && (!z.EED.HasValue || (z.EED > DateTime.Now)) && countryId == z.CountryId);

            return saleZonesBySellingNumberPlan;
        }

        public IEnumerable<SaleZoneInfo> GetSaleZonesInfo(string nameFilter, SaleZoneInfoFilter filter)
        {
            string nameFilterLower = nameFilter != null ? nameFilter.ToLower() : null;
            IEnumerable<SaleZone> saleZonesBySellingNumberPlan = GetSaleZones();
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

        public string GetSaleZoneName(long saleZoneId)
        {
            SaleZone saleZone = GetSaleZone(saleZoneId);

            if (saleZone != null)
                return saleZone.Name;

            return null;
        }

        public List<SaleZone> GetSaleZonesEffectiveAfter(int countryId, DateTime minimumDate)
        {
            ISaleZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleZoneDataManager>();
            return dataManager.GetSaleZonesEffectiveAfter(countryId, minimumDate);
        }


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

            int countryId = saleZone.CountryId;
            var country = manager.GetCountry(countryId);
            saleZoneDetail.CountryName = (country != null) ? country.Name : "";

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
            return new SaleZoneInfo { SaleZoneId = saleZone.SaleZoneId, Name = saleZone.Name };
        }

        #endregion
    }
}

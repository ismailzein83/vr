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
    public class SaleZoneManager
    {
        public List<SaleZone> GetSaleZones(int sellingNumberPlanId, DateTime effectiveDate)
        {
            return this.GetCachedSaleZones(sellingNumberPlanId);
        }

        public SaleZone GetSaleZone(long saleZoneId)
        {
            return GetAllSaleZones().GetRecord(saleZoneId);
        }

        public Dictionary<long, SaleZone> GetAllSaleZones()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllSaleZones", () =>
                {
                    ISaleZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleZoneDataManager>();
                    IEnumerable<SaleZone> allSaleZones = dataManager.GetAllSaleZones();
                    Dictionary<long, SaleZone> allSaleZonesDic = new Dictionary<long, SaleZone>();
                    if(allSaleZones != null)
                    {
                        foreach(var saleZone in allSaleZones)
                        {
                            allSaleZonesDic.Add(saleZone.SaleZoneId, saleZone);
                        }
                    }
                    return allSaleZonesDic;
                });
        }

        public Dictionary<string, List<SaleCode>> GetSaleZonesWithCodes(int sellingNumberPlanId,DateTime effectiveDate)
        {
            Dictionary<string, List<SaleCode>> saleZoneDictionary = new Dictionary<string, List<SaleCode>>();
            IEnumerable<SaleZone> salezones = this.GetSaleZones(sellingNumberPlanId, effectiveDate);

            if (salezones != null && salezones.Count() > 0)
            {
                SaleCodeManager manager = new SaleCodeManager();
                foreach (SaleZone saleZone in salezones)
                {

                    List<SaleCode> saleCodes = manager.GetSaleCodesByZoneID(saleZone.SaleZoneId, effectiveDate);
                    List<SaleCode> saleCodesOut;
                    if (!saleZoneDictionary.TryGetValue(saleZone.Name, out saleCodesOut))
                        saleZoneDictionary.Add(saleZone.Name, saleCodes);
                }
            }

            return saleZoneDictionary;
        }

        public string GetDescription(int sellingNumberPlanId, IEnumerable<long> saleZoneIds)
        {
            IEnumerable<SaleZone> allZones = GetCachedSaleZones(sellingNumberPlanId);

            Func<SaleZone, bool> filterExpression = null;

            if (saleZoneIds != null)
                filterExpression = (itm) => (saleZoneIds.Contains(itm.SaleZoneId));

            allZones = allZones.FindAllRecords(filterExpression);

            if (allZones != null)
                return string.Join(", ", allZones.Select(x => x.Name));

            return string.Empty;
        }

        public void InsertSaleZones(List<SaleZone> saleZones)
        {
            ISaleZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleZoneDataManager>();
            object dbApplyStream = dataManager.InitialiazeStreamForDBApply();
            foreach (SaleZone saleZone in saleZones)
                dataManager.WriteRecordToStream(saleZone, dbApplyStream);
            object prepareToApplySaleZones = dataManager.FinishDBApplyStream(dbApplyStream);
            dataManager.ApplySaleZonesForDB(prepareToApplySaleZones);
        }

        public void DeleteSaleZones(List<SaleZone> saleZones)
        {
            ISaleZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleZoneDataManager>();
            dataManager.DeleteSaleZones(saleZones);
        }

        public List<Vanrise.Entities.TemplateConfig> GetSaleZoneGroupTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.SaleZoneGroupConfigType);
        }

        //public IEnumerable<SaleZoneInfo> GetSaleZonesInfoByIds(int sellingNumberPlanId, List<long> saleZoneIds)
        //{
        //    List<SaleZone> allZones = GetCachedSaleZones(sellingNumberPlanId);
        //    return allZones.MapRecords(SaleZoneInfoMapper, itm => saleZoneIds.Contains(itm.SaleZoneId));
        //}

        public IEnumerable<SaleZone> GetSaleZonesByCountryIds(int sellingNumberPlanId, IEnumerable<int> countryIds)
        {
            IEnumerable<SaleZone> allSaleZones = GetCachedSaleZones(sellingNumberPlanId);
            
            if (allSaleZones != null)
                allSaleZones = allSaleZones.FindAllRecords(z => z.CountryId != null && countryIds.Contains((int)z.CountryId));

            return allSaleZones;
        }

        public IEnumerable<SaleZoneInfo> GetSaleZonesInfo(string nameFilter, SaleZoneInfoFilter filter)
        {
            string nameFilterLower = nameFilter != null ? nameFilter.ToLower() : null;
            List<SaleZone> allZones = GetCachedSaleZones(filter.SellingNumberPlanId);
            HashSet<long> filteredZoneIds = SaleZoneGroupContext.GetFilteredZoneIds(filter.SaleZoneFilterSettings);
            Func<SaleZone, bool> zoneFilter = (zone) =>
            {
                if (filteredZoneIds != null && !filteredZoneIds.Contains(zone.SaleZoneId))
                    return false;
                if (nameFilterLower != null && !zone.Name.Contains(nameFilterLower))
                    return false;
                return true;
            };
            return allZones.MapRecords(SaleZoneInfoMapper, zoneFilter);
        }

        public IEnumerable<SaleZoneInfo> GetSaleZonesInfoByIds(int sellingNumberPlanId, HashSet<long> saleZoneIds, SaleZoneFilterSettings saleZoneFilterSettings)
        {
            List<SaleZone> allZones = GetCachedSaleZones(sellingNumberPlanId);
            HashSet<long> filteredZoneIds = SaleZoneGroupContext.GetFilteredZoneIds(saleZoneFilterSettings);
            Func<SaleZone, bool> zoneFilter = (zone) =>
            {
                if (!saleZoneIds.Contains(zone.SaleZoneId))
                    return false;
                if (filteredZoneIds != null && !filteredZoneIds.Contains(zone.SaleZoneId))
                    return false;
                return true;
            };
            return allZones.MapRecords(SaleZoneInfoMapper, zoneFilter);
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
        
        private List<SaleZone> GetCachedSaleZones(int sellingNumberPlanId)
        {
            string cacheName = String.Format("GetCachedSaleZones_{0}", sellingNumberPlanId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName, () =>
            {
                ISaleZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleZoneDataManager>();
                return dataManager.GetSaleZones(sellingNumberPlanId);
            });
        }

        private SaleZoneDetail SaleZoneDetailMapper(SaleZone saleZone)
        {
            SaleZoneDetail saleZoneDetail = new SaleZoneDetail();

            saleZoneDetail.Entity = saleZone;

            CountryManager manager = new CountryManager();
            if (saleZone.CountryId != null)
            {
                int countryId = (int)saleZone.CountryId;
                saleZoneDetail.CountryName = manager.GetCountry(countryId).Name;
            }

            return saleZoneDetail;
        }

        private SaleZoneInfo SaleZoneInfoMapper(SaleZone saleZone)
        {
            return new SaleZoneInfo { SaleZoneId = saleZone.SaleZoneId, Name = saleZone.Name };
        }

        #endregion
    }
}

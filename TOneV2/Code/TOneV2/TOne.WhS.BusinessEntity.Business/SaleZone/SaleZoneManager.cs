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
        #region Private Classes
        
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISaleZoneDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISaleZoneDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return _dataManager.AreZonesUpdated(ref _updateHandle);
            }
        }

        #endregion

        public List<SaleZone> GetSaleZones(int sellingNumberPlanId)
        {
            ISaleZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleZoneDataManager>();
            return dataManager.GetSaleZones(sellingNumberPlanId);
        }

        public List<SaleZone> GetCachedSaleZones(int sellingNumberPlanId)
        {
            string cacheName = String.Format("GetCachedSaleZones_{0}", sellingNumberPlanId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName, () => GetSaleZones(sellingNumberPlanId));
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

        public List<SaleZone> GetSaleZones(int sellingNumberPlanId,DateTime effectiveDate)
        {
            ISaleZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleZoneDataManager>();
            return dataManager.GetSaleZones(sellingNumberPlanId,effectiveDate);
        }

        public Dictionary<string, List<SaleCode>> GetSaleZonesWithCodes(int sellingNumberPlanId,DateTime effectiveDate)
        {
            Dictionary<string, List<SaleCode>> saleZoneDictionary = new Dictionary<string, List<SaleCode>>();
            List<SaleZone> salezones = GetSaleZones(sellingNumberPlanId, effectiveDate);
            if (salezones != null && salezones.Count>0)
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

        public IEnumerable<SaleZoneInfo> GetSaleZonesInfo(int sellingNumberPlanId, string filter)
        {
            string filterLower = filter != null ? filter.ToLower() : null;
            List<SaleZone> allZones = GetCachedSaleZones(sellingNumberPlanId);
            if (allZones != null)
                return allZones.Where(itm => filterLower == null || itm.Name.Contains(filterLower)).Select(itm => new SaleZoneInfo { SaleZoneId = itm.SaleZoneId, Name = itm.Name });
            else
                return null;
        }

        public IEnumerable<SaleZoneInfo> GetSaleZonesInfoByIds(int sellingNumberPlanId, List<long> saleZoneIds)
        {
            List<SaleZone> allZones = GetCachedSaleZones(sellingNumberPlanId);
            if (allZones != null)
                return allZones.Where(itm => saleZoneIds.Contains(itm.SaleZoneId)).Select(itm => new SaleZoneInfo { SaleZoneId = itm.SaleZoneId, Name = itm.Name });
            else
                return null;
        }

        public IEnumerable<SaleZone> GetSaleZonesByIds(int sellingNumberPlanId, List<long> saleZoneIds)
        {
            List<SaleZone> allZones = GetCachedSaleZones(sellingNumberPlanId);

            if (allZones != null)
                return allZones.Where(item => saleZoneIds.Contains(item.SaleZoneId));

            return null;
        }
    }
}

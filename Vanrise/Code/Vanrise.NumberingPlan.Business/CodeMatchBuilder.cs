using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.NumberingPlan.Entities;
using Vanrise.Common;

namespace Vanrise.NumberingPlan.Business
{
    public class CodeMatchBuilder
    {
        public SaleCodeMatch GetMatchPlanSaleCode(string number, DateTime effectiveOn)
        {
            SaleCodeMatch codeMatch = new SaleCodeMatch();

            SellingNumberPlanManager numberPlanManager = new SellingNumberPlanManager();
            var masterNumberPlan = numberPlanManager.GetSellingNumberPlan(0); // GetMasterSellingNumberPlan
            if (masterNumberPlan == null)
                throw new NullReferenceException("masterNumberPlan");

            SaleCodeIterator masterCodeIterator = GetSellingNumberPlanSaleCodeIterator(masterNumberPlan.SellingNumberPlanId, effectiveOn);
            if (masterCodeIterator != null)
                codeMatch = masterCodeIterator.GetCodeMatch(number, false);

            //if (customerId.HasValue)
            //{
            //    var carrierAccountManager = new CarrierAccountManager();
            //    var customerNumberPlanId = carrierAccountManager.GetSellingNumberPlanId(customerId.Value);
            //    codeMatch.CustomerSellingNumberPlanId = customerNumberPlanId;
            //    if (customerNumberPlanId == masterNumberPlan.SellingNumberPlanId)
            //        codeMatch.SaleCodeMatch = codeMatch.MasterPlanCodeMatch;
            //    else
            //    {
            //        var saleCodeIterator = GetSellingNumberPlanSaleCodeIterator(customerNumberPlanId, effectiveOn);
            //        if (saleCodeIterator != null)
            //            codeMatch.SaleCodeMatch = saleCodeIterator.GetCodeMatch(number, false);
            //    }
            //}

            return codeMatch;
        }

        private struct GetSellingNumberPlanSaleCodeIteratorCacheName : IBEDayFilterCacheName
        {
            public int SettingNumberPlanId { get; set; }

            public DateTime EffectiveOn { get; set; }

            public DateTime FilterDay
            {
                get { return EffectiveOn; }
            }
        }

        private SaleCodeIterator GetSellingNumberPlanSaleCodeIterator(int sellingNumberPlanId, DateTime effectiveOn)
        {
            var cacheName = new GetSellingNumberPlanSaleCodeIteratorCacheName { SettingNumberPlanId = sellingNumberPlanId, EffectiveOn = effectiveOn.Date };// String.Concat("GetSellingNumberPlanSaleCodeIterator_", sellingNumberPlanId, "_", effectiveOn.Date);
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleCodeCacheManager>();
            return cacheManager.GetOrCreateObject(cacheName,
                () =>
                {
                    var codesByNumberPlans = GetCachedSaleCodes(effectiveOn);
                    var numberPlanSaleCodes = codesByNumberPlans.GetRecord(sellingNumberPlanId);

                    if (numberPlanSaleCodes != null)
                    {
                        return new SaleCodeIterator()
                        {
                            CodeIterator = new CodeIterator<SaleCode>(numberPlanSaleCodes),
                            SellingNumberPlanId = sellingNumberPlanId
                        };
                    }
                    else
                        return null;
                });
        }

        private struct GetCachedSaleCodesCacheName : IBEDayFilterCacheName
        {
            public DateTime EffectiveOn { get; set; }

            public DateTime FilterDay
            {
                get { return EffectiveOn; }
            }
        }

        private Dictionary<int, List<SaleCode>> GetCachedSaleCodes(DateTime effectiveOn)
        {
            var cacheName = new GetCachedSaleCodesCacheName { EffectiveOn = effectiveOn.Date };
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleCodeCacheManager>();
            return cacheManager.GetOrCreateObject(cacheName, () =>
            {
                var rslt = new Dictionary<int, List<SaleCode>>();
                SaleCodeManager saleCodeManager = new SaleCodeManager();
                List<SaleCode> allSaleCodes = saleCodeManager.GetSaleCodes(effectiveOn);
                var zones = new SaleZoneManager().GetCachedSaleZones();
                foreach (var code in allSaleCodes)
                {
                    var cachedCode = cacheManager.CacheAndGetCode(code);
                    var zone = zones.GetRecord(cachedCode.ZoneId);
                    rslt.GetOrCreateItem(zone.SellingNumberPlanId).Add(cachedCode);
                }
                return rslt;
            });
        }
    }
}

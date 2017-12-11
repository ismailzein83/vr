using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Caching;
using Vanrise.Caching.Runtime;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Business
{
    public class CodeMatchBuilder
    {
        #region Public Methods

        public void BuildCodeMatches(IBuildCodeMatchesContext context, IEnumerable<SaleCode> saleCodes, IEnumerable<SupplierCode> supplierCodes, Action<CodeMatches> onCodeMatchesAvailable)
        {
            List<SaleCodeIterator> saleCodeIterators;
            HashSet<string> distinctSaleCodes;
            List<SupplierCodeIterator> supplierCodeIterators;
            HashSet<string> distinctSupplierCodes;
            StructuresSaleCodes(saleCodes, out saleCodeIterators, out distinctSaleCodes);
            StructuresSupplierCodes(supplierCodes, out supplierCodeIterators, out distinctSupplierCodes);

            foreach (string code in distinctSupplierCodes)
            {
                if (context.CodePrefix.Length > code.Length)
                    continue;

                BuildAndAddCodeMatches(code, false, saleCodeIterators, supplierCodeIterators, context.SupplierZoneDetails, onCodeMatchesAvailable);
            }
            foreach (string code in distinctSaleCodes)
            {
                if (distinctSupplierCodes.Contains(code) || context.CodePrefix.Length > code.Length)
                    continue;

                BuildAndAddCodeMatches(code, true, saleCodeIterators, supplierCodeIterators, context.SupplierZoneDetails, onCodeMatchesAvailable);
            }
        }

        public SaleCodeMatch GetSaleCodeMatch(string number, int customerId, DateTime effectiveOn)
        {
            SaleCodeIterator saleCodeIterator = GetCustomerSaleCodeIterator(customerId, effectiveOn);
            if (saleCodeIterator != null)
                return saleCodeIterator.GetCodeMatch(number, false);
            else
                return null;
        }

        public SaleCodeMatchWithMaster GetSaleCodeMatchWithMaster(string number, int? customerId, DateTime effectiveOn)
        {
            SaleCodeMatchWithMaster codeMatch = new SaleCodeMatchWithMaster();
            SellingNumberPlanManager numberPlanManager = new SellingNumberPlanManager();
            var masterNumberPlan = numberPlanManager.GetMasterSellingNumberPlan();
            if (masterNumberPlan == null)
                throw new NullReferenceException("masterNumberPlan");

            SaleCodeIterator masterCodeIterator = GetSellingNumberPlanSaleCodeIterator(masterNumberPlan.SellingNumberPlanId, effectiveOn);
            if (masterCodeIterator != null)
                codeMatch.MasterPlanCodeMatch = masterCodeIterator.GetCodeMatch(number, false);
            if (customerId.HasValue)
            {
                var carrierAccountManager = new CarrierAccountManager();
                var customerNumberPlanId = carrierAccountManager.GetSellingNumberPlanId(customerId.Value);
                codeMatch.CustomerSellingNumberPlanId = customerNumberPlanId;
                if (customerNumberPlanId == masterNumberPlan.SellingNumberPlanId)
                    codeMatch.SaleCodeMatch = codeMatch.MasterPlanCodeMatch;
                else
                {
                    var saleCodeIterator = GetSellingNumberPlanSaleCodeIterator(customerNumberPlanId, effectiveOn);
                    if (saleCodeIterator != null)
                        codeMatch.SaleCodeMatch = saleCodeIterator.GetCodeMatch(number, false);
                }
            }
            return codeMatch;
        }

        public SupplierCodeMatch GetSupplierCodeMatch(string number, int supplierId, DateTime effectiveOn)
        {
            SupplierCodeIteratorInfo supplierCodeIteratorInfo = GetSupplierCodeIterator(supplierId, effectiveOn);
            if (supplierCodeIteratorInfo != null && supplierCodeIteratorInfo.SupplierCodeIterator != null)
                return supplierCodeIteratorInfo.SupplierCodeIterator.GetCodeMatch(number);
            else
                return null;
        }

        #endregion

        #region Private Methods

        private void StructuresSaleCodes(IEnumerable<SaleCode> saleCodes, out List<SaleCodeIterator> saleCodeIterators, out HashSet<string> distinctSaleCodes)
        {
            saleCodeIterators = new List<SaleCodeIterator>();
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            distinctSaleCodes = new HashSet<string>();
            Dictionary<int, List<SaleCode>> saleCodesBySellingNumberPlan = new Dictionary<int, List<SaleCode>>();
            foreach (var saleCode in saleCodes)
            {
                distinctSaleCodes.Add(saleCode.Code);
                SaleZone saleZone = saleZoneManager.GetSaleZone(saleCode.ZoneId);
                List<SaleCode> currentSaleCodes = saleCodesBySellingNumberPlan.GetOrCreateItem(saleZone.SellingNumberPlanId);
                currentSaleCodes.Add(saleCode);
            }
            foreach (var saleCodeEntry in saleCodesBySellingNumberPlan)
            {
                SaleCodeIterator codeIterator = new SaleCodeIterator
                {
                    SellingNumberPlanId = saleCodeEntry.Key,
                    CodeIterator = new CodeIterator<SaleCode>(saleCodeEntry.Value)
                };
                saleCodeIterators.Add(codeIterator);
            }
        }

        private void StructuresSupplierCodes(IEnumerable<SupplierCode> supplierCodes, out List<SupplierCodeIterator> supplierCodeIterators, out HashSet<string> distinctSupplierCodes)
        {
            supplierCodeIterators = new List<SupplierCodeIterator>();
            SupplierZoneManager supplierZoneManager = new SupplierZoneManager();
            distinctSupplierCodes = new HashSet<string>();
            Dictionary<int, List<SupplierCode>> supplierCodesBySupplier = new Dictionary<int, List<SupplierCode>>();
            foreach (var supplierCode in supplierCodes)
            {
                distinctSupplierCodes.Add(supplierCode.Code);
                SupplierZone supplierZone = supplierZoneManager.GetSupplierZone(supplierCode.ZoneId);
                List<SupplierCode> currentSupplierCodes = supplierCodesBySupplier.GetOrCreateItem(supplierZone.SupplierId);
                currentSupplierCodes.Add(supplierCode);
            }
            foreach (var supplierCodeEntry in supplierCodesBySupplier)
            {
                SupplierCodeIterator codeIterator = new SupplierCodeIterator
                {
                    SupplierId = supplierCodeEntry.Key,
                    CodeIterator = new CodeIterator<SupplierCode>(supplierCodeEntry.Value)
                };
                supplierCodeIterators.Add(codeIterator);
            }
        }

        private void BuildAndAddCodeMatches(string code, bool isDistinctFromSaleCodes, List<SaleCodeIterator> saleCodeIterators, List<SupplierCodeIterator> supplierCodeIterators, SupplierZoneDetailByZone supplierZoneDetailsByZone, Action<CodeMatches> onCodeMatchesAvailable)
        {
            List<SaleCodeMatch> saleCodeMatches = new List<SaleCodeMatch>();
            foreach (var saleCodeIterator in saleCodeIterators)
            {
                var saleCodeMatch = saleCodeIterator.GetCodeMatch(code, isDistinctFromSaleCodes);
                if (saleCodeMatch != null)
                    saleCodeMatches.Add(saleCodeMatch);
            }

            if (saleCodeMatches.Count > 0 || !isDistinctFromSaleCodes)
            {
                List<SupplierCodeMatchWithRate> supplierCodeMatches = new List<SupplierCodeMatchWithRate>();
                SupplierCodeMatchWithRateBySupplier supplierCodeMatchBySupplier = new SupplierCodeMatchWithRateBySupplier();
                foreach (var supplierCodeIterator in supplierCodeIterators)
                {
                    SupplierCodeMatch supplierCodeMatch = supplierCodeIterator.GetCodeMatch(code);
                    if (supplierCodeMatch != null)
                    {
                        SupplierZoneDetail supplierZoneDetail;
                        if (supplierZoneDetailsByZone.TryGetValue(supplierCodeMatch.SupplierZoneId, out supplierZoneDetail))
                        {
                            if (supplierZoneDetail == null)
                            {
                                //TODO: log a business error here
                                continue;
                            }

                            SupplierCodeMatchWithRate supplierCodeMatchWithRate = new SupplierCodeMatchWithRate
                            {
                                CodeMatch = supplierCodeMatch,
                                RateValue = supplierZoneDetail.EffectiveRateValue,
                                SupplierServiceIds = supplierZoneDetail.SupplierServiceIds,
                                ExactSupplierServiceIds = supplierZoneDetail.ExactSupplierServiceIds,
                                SupplierServiceWeight = supplierZoneDetail.SupplierServiceWeight,
                                SupplierRateId = supplierZoneDetail.SupplierRateId,
                                SupplierRateEED = supplierZoneDetail.SupplierRateEED
                            };
                            supplierCodeMatches.Add(supplierCodeMatchWithRate);
                            supplierCodeMatchBySupplier.Add(supplierCodeIterator.SupplierId, supplierCodeMatchWithRate);
                        }
                    }
                }
                CodeMatches codeMatches = new CodeMatches
                {
                    Code = code,
                    SaleCodeMatches = saleCodeMatches,
                    SupplierCodeMatches = supplierCodeMatches,
                    SupplierCodeMatchesBySupplier = supplierCodeMatchBySupplier
                };
                onCodeMatchesAvailable(codeMatches);
            }
        }

        private SaleCodeIterator GetCustomerSaleCodeIterator(int customerId, DateTime effectiveOn)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            int sellingNumberPlanId = carrierAccountManager.GetSellingNumberPlanId(customerId);
            return GetSellingNumberPlanSaleCodeIterator(sellingNumberPlanId, effectiveOn);

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

        private struct GetSupplierCodeIteratorCacheName : IBEDayFilterCacheName
        {
            public int SupplierId { get; set; }

            public DateTime EffectiveOn { get; set; }

            public DateTime FilterDay
            {
                get { return this.EffectiveOn; }
            }
        }

        private SupplierCodeIteratorInfo GetSupplierCodeIterator(int supplierId, DateTime effectiveOn)
        {
            List<SupplierCodeIteratorInfo> supplierCodeIteratorInfos = GetSupplierCodeIterators(supplierId, effectiveOn);
            return TOne.WhS.BusinessEntity.Business.Helper.GetBusinessEntityInfo<SupplierCodeIteratorInfo>(supplierCodeIteratorInfos, effectiveOn);
        }

        private List<SupplierCodeIteratorInfo> GetSupplierCodeIterators(int supplierId, DateTime effectiveOn)
        {
            DateTimeRange dateTimeRange = TOne.WhS.BusinessEntity.Business.Helper.GetDateTimeRangeWithOffset(effectiveOn);

            var cacheName = new GetSupplierCodeIteratorCacheName { SupplierId = supplierId, EffectiveOn = dateTimeRange.From };// String.Concat("GetSupplierCodeIterator_", supplierId, "_", effectiveOn.Date);
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierCodeCacheManager>();

            return cacheManager.GetOrCreateObject(cacheName,
               () =>
               {
                   Dictionary<int, List<SupplierCode>> codesBySupplier = GetCachedSupplierCodes(dateTimeRange.From, dateTimeRange.To);

                   var supplierCodes = codesBySupplier.GetRecord(supplierId);
                   if (supplierCodes == null)
                       return null;

                   List<SupplierCodeIteratorInfo> supplierCodeIterators = new List<SupplierCodeIteratorInfo>();

                   TOne.WhS.BusinessEntity.Business.Helper.StructureBusinessEntitiesByDate(supplierCodes, dateTimeRange.From, dateTimeRange.To, (matchingSupplierCodes, bed, eed) =>
                   {
                       SupplierCodeIteratorInfo supplierCodeIteratorInfo = new SupplierCodeIteratorInfo()
                       {
                           BED = bed,
                           EED = eed,
                           SupplierCodeIterator = new SupplierCodeIterator()
                           {
                               CodeIterator = new CodeIterator<SupplierCode>(matchingSupplierCodes),
                               SupplierId = supplierId
                           }
                       };
                       supplierCodeIterators.Add(supplierCodeIteratorInfo);
                   });
                   return supplierCodeIterators;
               });
        }

        private struct GetCachedSupplierCodesCacheName : IBEDayFilterCacheName
        {
            public DateTime EffectiveOn { get; set; }

            public DateTime FilterDay
            {
                get { return this.EffectiveOn; }
            }
        }

        private Dictionary<int, List<SupplierCode>> GetCachedSupplierCodes(DateTime from, DateTime to)
        {
            var cacheName = new GetCachedSupplierCodesCacheName { EffectiveOn = from };
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierCodeCacheManager>();
            return cacheManager.GetOrCreateObject(cacheName, () =>
             {
                 var rslt = new Dictionary<int, List<SupplierCode>>();
                 SupplierCodeManager supplierCodeManager = new SupplierCodeManager();
                 List<SupplierCode> allSupplierCodes = supplierCodeManager.GetSupplierCodes(from, to);
                 var zones = new SupplierZoneManager().GetCachedSupplierZones();
                 foreach (var code in allSupplierCodes)
                 {
                     var cachedCode = cacheManager.CacheAndGetCode(code);
                     var zone = zones.GetRecord(cachedCode.ZoneId);
                     rslt.GetOrCreateItem(zone.SupplierId).Add(cachedCode);
                 }
                 return rslt;
             });
        }

        #endregion
    }
}

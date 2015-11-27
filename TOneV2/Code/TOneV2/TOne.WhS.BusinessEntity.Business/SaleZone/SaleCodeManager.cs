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
    public class SaleCodeManager
    {
        public Vanrise.Entities.IDataRetrievalResult<SaleCodeDetail> GetFilteredSaleCodes(Vanrise.Entities.DataRetrievalInput<SaleCodeQuery> input)
        {
            var allSaleCodes = GetAllSaleCodes();
            Func<SaleCode, bool> filterExpression = (prod) =>
                     (input.Query.Code == null || prod.Code.Contains(input.Query.Code))
                  && (input.Query.CodeGroupId.Equals(prod.CodeGroupId))
                  && ((!input.Query.EffectiveOn.HasValue || (prod.BeginEffectiveDate <= input.Query.EffectiveOn)))
                  && ((!input.Query.EffectiveOn.HasValue || !prod.EndEffectiveDate.HasValue || (prod.EndEffectiveDate > input.Query.EffectiveOn)));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allSaleCodes.ToBigResult(input, filterExpression, SaleCodeDetailMapper));
        }

        public Dictionary<long, SaleCode> GetAllSaleCodes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllSaleZones", () =>
            {
                ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
                IEnumerable<SaleCode> allSaleCodes = dataManager.GetAllSaleCodes();
                Dictionary<long, SaleCode> allSaleCodesDic = new Dictionary<long, SaleCode>();
                if (allSaleCodes != null)
                {
                    foreach (var saleCode in allSaleCodes)
                    {
                        allSaleCodesDic.Add(saleCode.SaleCodeId, saleCode);
                    }
                }
                return allSaleCodesDic;
            });
        }
        public List<SaleCode> GetSaleCodesByZoneID(long zoneID,DateTime effectiveDate)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesByZoneID(zoneID, effectiveDate);
        }
    

        public List<SaleCode> GetSellingNumberPlanSaleCodes(int sellingNumberPlanId, DateTime effectiveOn)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
           return dataManager.GetSellingNumberPlanSaleCodes(sellingNumberPlanId, effectiveOn);
        }

        public List<SaleCode> GetSaleCodesByPrefix(string codePrefix, DateTime? effectiveOn, bool isFuture, bool getChildCodes, bool getParentCodes)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesByPrefix(codePrefix, effectiveOn, isFuture, getChildCodes, getParentCodes);
        }

        public IEnumerable<string> GetDistinctCodeByPrefixes(int prefixLength, DateTime? effectiveOn, bool isFuture)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetDistinctCodeByPrefixes(prefixLength, effectiveOn, isFuture);
        }
        #region private Methode
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISaleCodeDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return _dataManager.AreZonesUpdated(ref _updateHandle);
            }
        }

        private SaleCodeDetail SaleCodeDetailMapper(SaleCode saleCode)
        {
            SaleCodeDetail saleCodeDetail = new SaleCodeDetail();

            saleCodeDetail.Entity = saleCode;

            return saleCodeDetail;
        }

        #endregion

    }
}

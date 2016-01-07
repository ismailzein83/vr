using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleCodeManager
    {
        public Vanrise.Entities.IDataRetrievalResult<SaleCodeDetail> GetFilteredSaleCodes(Vanrise.Entities.DataRetrievalInput<SaleCodeQuery> input)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            Vanrise.Entities.BigResult<SaleCode> saleCodes = dataManager.GetSaleCodeFilteredFromTemp(input);
            BigResult<SaleCodeDetail> customerRouteDetailResult = new BigResult<SaleCodeDetail>()
            {
                ResultKey = saleCodes.ResultKey,
                TotalCount = saleCodes.TotalCount,
                Data = saleCodes.Data.MapRecords(SaleCodeDetailMapper)
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, customerRouteDetailResult);
        }

        public List<SaleCode> GetSaleCodesByZoneID(long zoneID, DateTime effectiveDate)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesByZoneID(zoneID, effectiveDate);
        }

        public long ReserveIDRange(int numberOfIDs)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(this.GetType(), numberOfIDs, out startingId);
            return startingId;
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

        public List<SaleCode> GetSaleCodesByZoneName(int sellingNumberPlanId, string zoneName, DateTime effectiveDate)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesByZoneName(sellingNumberPlanId, zoneName, effectiveDate);
        }

        public List<SaleCode> GetSaleCodesByCountry(int countryId, DateTime effectiveDate)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesByCountry(countryId, effectiveDate);
        }

        public List<SaleCode> GetSaleCodesEffectiveAfter(int sellingNumberPlanId, int countryId, DateTime minimumDate)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesEffectiveAfter(sellingNumberPlanId, countryId, minimumDate);
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
            SaleZoneManager szmnager = new SaleZoneManager();

            saleCodeDetail.Entity = saleCode;
            saleCodeDetail.ZoneName = szmnager.GetSaleZone(saleCode.ZoneId).Name;
            return saleCodeDetail;
        }

        #endregion

    }
}

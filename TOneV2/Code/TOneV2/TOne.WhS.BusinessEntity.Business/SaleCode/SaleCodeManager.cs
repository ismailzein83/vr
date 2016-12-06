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

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<SaleCodeDetail> GetFilteredSaleCodes(Vanrise.Entities.DataRetrievalInput<BaseSaleCodeQueryHandler> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SaleCodeRequestHandler());
        }

        public List<SaleCode> GetSaleCodesByZoneID(long zoneID, DateTime effectiveDate)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesByZoneID(zoneID, effectiveDate);
        }

        public List<SaleCode> GetSaleCodesByCodeGroups(List<int> codeGroupsIds)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesByCodeGroups(codeGroupsIds);
        }

        public List<SaleCode> GetSaleCodesEffectiveByZoneID(long zoneID, DateTime effectiveDate)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesEffectiveByZoneID(zoneID, effectiveDate);
        }

        public long ReserveIDRange(int numberOfIDs)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(GetSaleCodeType(), numberOfIDs, out startingId);
            return startingId;
        }
        public List<SaleCode> GetSaleCodes(DateTime effectiveOn)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodes(effectiveOn);
        }

        public List<SaleCode> GetSaleCodesEffectiveAfter(int sellingNumberPlanId, DateTime effectiveOn)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesEffectiveAfter(sellingNumberPlanId, effectiveOn);
        }

        
        public List<SaleCode> GetSaleCodesByPrefix(string codePrefix, DateTime? effectiveOn, bool isFuture, bool getChildCodes, bool getParentCodes)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesByPrefix(codePrefix, effectiveOn, isFuture, getChildCodes, getParentCodes);
        }

        public IEnumerable<CodePrefixInfo> GetDistinctCodeByPrefixes(int prefixLength, DateTime? effectiveOn, bool isFuture)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetDistinctCodeByPrefixes(prefixLength, effectiveOn, isFuture);
        }
        public IEnumerable<CodePrefixInfo> GetSpecificCodeByPrefixes(int prefixLength, IEnumerable<string> codePrefixes, DateTime? effectiveOn, bool isFuture)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSpecificCodeByPrefixes(prefixLength, codePrefixes, effectiveOn, isFuture);
        }

        public List<SaleCode> GetSaleCodesByZoneName(int sellingNumberPlanId, string zoneName, DateTime effectiveDate)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesByZoneName(sellingNumberPlanId, zoneName, effectiveDate);
        }
        public List<SaleCode> GetSaleCodesEffectiveAfter(int sellingNumberPlanId, int countryId, DateTime minimumDate)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesEffectiveAfter(sellingNumberPlanId, countryId, minimumDate);
        }

        public List<SaleCode> GetSaleCodesByZoneIDs(List<long> zoneIds, DateTime effectiveDate)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesByZoneIDs(zoneIds, effectiveDate);
        }

        public int GetSaleCodeTypeId()
        {
            return Vanrise.Common.Business.TypeManager.Instance.GetTypeId(this.GetSaleCodeType());
        }

        public Type GetSaleCodeType()
        {
            return this.GetType();
        }

        #endregion

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

        #region Private Classes

        private class SaleCodeRequestHandler : BigDataRequestHandler<BaseSaleCodeQueryHandler, SaleCode, SaleCodeDetail>
        {
            public override SaleCodeDetail EntityDetailMapper(SaleCode entity)
            {
                SaleCodeManager manager = new SaleCodeManager();
                return manager.SaleCodeDetailMapper(entity);
            }

            public override IEnumerable<SaleCode> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<BaseSaleCodeQueryHandler> input)
            {
                return input.Query.GetFilteredSaleCodes();
            }
        }

        #endregion

    }
}

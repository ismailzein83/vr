using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierCodeManager
    {
      
        #region Public Methods
        public List<SupplierCode> GetSupplierCodesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            ISupplierCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierCodeDataManager>();
            return dataManager.GetSupplierCodesEffectiveAfter(supplierId, minimumDate);
        }
        public List<SupplierCode> GetSupplierCodes(DateTime from, DateTime to)
        {
            ISupplierCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierCodeDataManager>();
            return dataManager.GetSupplierCodes(from, to);
        }

        public IEnumerable<CodePrefixInfo> GetDistinctCodeByPrefixes(int prefixLength, DateTime? effectiveOn, bool isFuture)
        {
            ISupplierCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierCodeDataManager>();
            return dataManager.GetDistinctCodeByPrefixes(prefixLength, effectiveOn, isFuture);
        }
        public IEnumerable<CodePrefixInfo> GetSpecificCodeByPrefixes(int prefixLength, IEnumerable<string> codePrefixes, DateTime? effectiveOn, bool isFuture)
        {
            ISupplierCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierCodeDataManager>();
            return dataManager.GetSpecificCodeByPrefixes(prefixLength, codePrefixes, effectiveOn, isFuture);
        }


        public Vanrise.Entities.IDataRetrievalResult<SupplierCodeDetail> GetFilteredSupplierCodes(Vanrise.Entities.DataRetrievalInput<SupplierCodeQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SupplierCodeRequestHandler());
        }

        public long ReserveIDRange(int numberOfIDs)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(GetSupplierCodeType(), numberOfIDs, out startingId);
            return startingId;
        }

        public int GetSupplierCodeTypeId()
        {
            return Vanrise.Common.Business.TypeManager.Instance.GetTypeId(this.GetSupplierCodeType());
        }

        public Type GetSupplierCodeType()
        {
           return this.GetType();
        }

        #endregion
       
        #region Private Members
        private SupplierCodeDetail SupplierCodeDetailMapper(SupplierCode supplierCode)
        {
            return new SupplierCodeDetail()
            {
                Entity = supplierCode,
                SupplierZoneName = this.GetSupplierZoneName(supplierCode.ZoneId),
            };
        }
        private string GetSupplierZoneName(long zoneId)
        {
            SupplierZoneManager manager = new SupplierZoneManager();
            SupplierZone suplierZone = manager.GetSupplierZone(zoneId);

            if (suplierZone != null)
                return suplierZone.Name;

            return "Zone Not Found";
        }
        #endregion

        #region Private Classes

        private class SupplierCodeRequestHandler : BigDataRequestHandler<SupplierCodeQuery, SupplierCode, SupplierCodeDetail>
        {
            public override SupplierCodeDetail EntityDetailMapper(SupplierCode entity)
            {
                SupplierCodeManager manager = new SupplierCodeManager();
                return manager.SupplierCodeDetailMapper(entity);
            }

            public override IEnumerable<SupplierCode> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SupplierCodeQuery> input)
            {
                ISupplierCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierCodeDataManager>();
                return dataManager.GetFilteredSupplierCodes(input.Query);
            }
        }

        #endregion


        public void LoadSupplierCodes(IEnumerable<RoutingSupplierInfo> activeSupplierInfo, string codePrefix, DateTime? effectiveOn, bool isFuture, Action<SupplierCode> onCodeLoaded)
        {
            ISupplierCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierCodeDataManager>();
            dataManager.LoadSupplierCodes(activeSupplierInfo, codePrefix, effectiveOn, isFuture, onCodeLoaded);
        }
    }
}

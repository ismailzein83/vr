using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierCodeManager
    {
        public List<SupplierCode> GetSupplierCodesEffectiveAfter(int supplierId, DateTime? minimumDate)
        {
            ISupplierCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierCodeDataManager>();
            return dataManager.GetSupplierCodesEffectiveAfter(supplierId, minimumDate);
        }

        public List<SupplierCode> GetSupplierCodes(int supplierId, DateTime effectiveOn)
        {
            ISupplierCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierCodeDataManager>();
            return dataManager.GetSupplierCodes(supplierId, effectiveOn);
        }

        public List<SupplierCode> GetActiveSupplierCodesByPrefix(string codePrefix, DateTime? effectiveOn, bool isFuture, bool getChildCodes, bool getParentCodes, IEnumerable<RoutingSupplierInfo> supplierInfo)
        {
            ISupplierCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierCodeDataManager>();
            return dataManager.GetActiveSupplierCodesByPrefix(codePrefix, effectiveOn, isFuture, getChildCodes, getParentCodes, supplierInfo);
        }

        public IEnumerable<string> GetDistinctCodeByPrefixes(int prefixLength, DateTime? effectiveOn, bool isFuture)
        {
            ISupplierCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierCodeDataManager>();
            return dataManager.GetDistinctCodeByPrefixes(prefixLength, effectiveOn, isFuture);
        }


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

        public Vanrise.Entities.IDataRetrievalResult<SupplierCodeDetail> GetFilteredSupplierCodes(Vanrise.Entities.DataRetrievalInput<SupplierCodeQuery> input)
        {
            ISupplierCodeDataManager manager = BEDataManagerFactory.GetDataManager<ISupplierCodeDataManager>();

            BigResult<SupplierCode> supplierCodeResult = manager.GetFilteredSupplierCodes(input);

            BigResult<SupplierCodeDetail> supplierCodeDetailResult = new BigResult<SupplierCodeDetail>()
            {
                ResultKey = supplierCodeResult.ResultKey,
                TotalCount = supplierCodeResult.TotalCount,
                Data = supplierCodeResult.Data.MapRecords(SupplierCodeDetailMapper)
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, supplierCodeDetailResult);
        }

    }
}

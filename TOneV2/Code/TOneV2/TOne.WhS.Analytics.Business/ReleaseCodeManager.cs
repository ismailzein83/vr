using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Data;
using TOne.WhS.Analytics.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.Analytics.Business
{
    public class ReleaseCodeManager
    {
        private readonly CarrierAccountManager _carrierAccountManager;
        private readonly SaleZoneManager _saleZoneManager;

        public ReleaseCodeManager()
        {
            _carrierAccountManager = new CarrierAccountManager();
            _saleZoneManager = new SaleZoneManager();
        }
        public Vanrise.Entities.IDataRetrievalResult<ReleaseCodeDetail> GetAllFilteredReleaseCodes(Vanrise.Entities.DataRetrievalInput<ReleaseCodeQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new ReleaseCodeRequestHandler());
        }

        public ReleaseCodeDetail ReleaseCodeDetailMapper(ReleaseCode releaseCode)
        {
            //string customerName = _carrierAccountManager.GetCarrierAccountName(ReleaseCode.CustomerId);
            //string supplierName = _carrierAccountManager.GetCarrierAccountName(ReleaseCode.SupplierId);
            //string saleZoneName = _saleZoneManager.GetSaleZoneName(ReleaseCode.SaleZoneId);

            ReleaseCodeDetail releaseCodeDetail = new ReleaseCodeDetail
            {
                Entity = releaseCode,
                //CustomerName = customerName,
                //SupplierName = supplierName,
                //SaleZoneName = saleZoneName
            };
            return releaseCodeDetail;
        }

        #region Private Classes

        private class ReleaseCodeRequestHandler : BigDataRequestHandler<ReleaseCodeQuery, ReleaseCode, ReleaseCodeDetail>
        {
            public override ReleaseCodeDetail EntityDetailMapper(ReleaseCode entity)
            {
                ReleaseCodeManager manager = new ReleaseCodeManager();
                return manager.ReleaseCodeDetailMapper(entity);
            }

            public override IEnumerable<ReleaseCode> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<ReleaseCodeQuery> input)
            {
                IReleaseCodeDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<IReleaseCodeDataManager>();
                return dataManager.GetAllFilteredReleaseCodes(input);
            }
        }

        #endregion
    }
}

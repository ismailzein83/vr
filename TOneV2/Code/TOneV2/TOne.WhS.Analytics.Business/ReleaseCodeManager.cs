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
        private readonly SwitchManager _switchManager;
      
        public ReleaseCodeManager()
        {
            _carrierAccountManager = new CarrierAccountManager();
            _saleZoneManager = new SaleZoneManager();
            _switchManager = new SwitchManager();
        }
        public Vanrise.Entities.IDataRetrievalResult<ReleaseCodeStatDetail> GetAllFilteredReleaseCodes(Vanrise.Entities.DataRetrievalInput<ReleaseCodeQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new ReleaseCodeRequestHandler());
        }

        public ReleaseCodeStatDetail ReleaseCodeDetailMapper(ReleaseCodeStat releaseCode)
        {
            
            string supplierName = _carrierAccountManager.GetCarrierAccountName(releaseCode.SupplierId);
            string switchName = _switchManager.GetSwitchName(releaseCode.SwitchId);
            string zoneName = _saleZoneManager.GetSaleZoneName(releaseCode.MasterPlanZoneId);
            ReleaseCodeStatDetail releaseCodeDetail = new ReleaseCodeStatDetail
            {
                Entity = releaseCode,
                SupplierName = supplierName,
                SwitchName = switchName,
                ZoneName = zoneName
            };
            return releaseCodeDetail;
        }

        #region Private Classes

        private class ReleaseCodeRequestHandler : BigDataRequestHandler<ReleaseCodeQuery, ReleaseCodeStat, ReleaseCodeStatDetail>
        {
            public override ReleaseCodeStatDetail EntityDetailMapper(ReleaseCodeStat entity)
            {
                ReleaseCodeManager manager = new ReleaseCodeManager();
                return manager.ReleaseCodeDetailMapper(entity);
            }

            public override IEnumerable<ReleaseCodeStat> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<ReleaseCodeQuery> input)
            {
                IReleaseCodeDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<IReleaseCodeDataManager>();
                SaleCodeManager manager = new SaleCodeManager();
                List<string> salecodesIds = new List<string>();
                List<SaleCode> salecodes = manager.GetSaleCodesByCodeGroups(input.Query.Filter.CodeGroupIds);
                salecodesIds = salecodes.Select(x => x.Code).ToList();

                return dataManager.GetAllFilteredReleaseCodes(input, salecodesIds);
            }
        }

        #endregion
    }
}

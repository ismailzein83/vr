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
    public class RepeatedNumberManager
    {
        private readonly CarrierAccountManager _carrierAccountManager;
        private readonly SwitchManager _switchManager;
        private readonly SaleZoneManager _saleZoneManager;

        public RepeatedNumberManager()
        {
            _carrierAccountManager = new CarrierAccountManager();
            _switchManager = new SwitchManager();
            _saleZoneManager = new SaleZoneManager();
        }
        public Vanrise.Entities.IDataRetrievalResult<RepeatedNumberDetail> GetRepeatedNumberData(Vanrise.Entities.DataRetrievalInput<RepeatedNumberInput> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new RepeatedNumberRequestHandler());
        }

        public RepeatedNumberDetail RepeatedNumberDetailMapper(RepeatedNumber repeatedNumber)
        {
            string customerName = _carrierAccountManager.GetCarrierAccountName(repeatedNumber.CustomerId);
            string supplierName = _carrierAccountManager.GetCarrierAccountName(repeatedNumber.SupplierId);
            string saleZoneName = _saleZoneManager.GetSaleZoneName(repeatedNumber.SaleZoneId);
            string switchName = _switchManager.GetSwitchName(repeatedNumber.SwitchId);

            RepeatedNumberDetail repeatedNumberDetail = new RepeatedNumberDetail
            {
                Entity = repeatedNumber,
                CustomerName = customerName,
                SupplierName = supplierName,
                SaleZoneName = saleZoneName,
                SwitchName = switchName,
            };
            return repeatedNumberDetail;
        }

        #region Private Classes

        private class RepeatedNumberRequestHandler : BigDataRequestHandler<RepeatedNumberInput, RepeatedNumber, RepeatedNumberDetail>
        {
            public override RepeatedNumberDetail EntityDetailMapper(RepeatedNumber entity)
            {
                RepeatedNumberManager manager = new RepeatedNumberManager();
                return manager.RepeatedNumberDetailMapper(entity);
            }

            public override IEnumerable<RepeatedNumber> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<RepeatedNumberInput> input)
            {
                IRepeatedNumberDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<IRepeatedNumberDataManager>();
                return dataManager.GetRepeatedNumberData(input);
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Data;
using TOne.WhS.Analytics.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;
namespace TOne.WhS.Analytics.Business
{
    public class CDRManager
    {
        private readonly CarrierAccountManager carrierAccountManager;
        private readonly SwitchManager switchManager;
        private readonly SaleZoneManager saleZoneManager;
        private readonly SupplierZoneManager supplierZoneManager;

        public CDRManager()
        {
            carrierAccountManager = new CarrierAccountManager();
            switchManager = new SwitchManager();
            saleZoneManager = new SaleZoneManager();
            supplierZoneManager = new SupplierZoneManager();
        }
        public Vanrise.Entities.IDataRetrievalResult<CDRLogDetail> GetCDRLogData(Vanrise.Entities.DataRetrievalInput<CDRLogInput> input)
        {
            ICDRDataManager _datamanager = AnalyticsDataManagerFactory.GetDataManager<ICDRDataManager>();
            var cdrLogResult = _datamanager.GetCDRLogData(input);
            BigResult<CDRLogDetail> CDRLogBigResultDetailMapper = new BigResult<CDRLogDetail>
            {
                Data = cdrLogResult.Data.MapRecords(CDRLogDetailMapper),
                ResultKey = cdrLogResult.ResultKey,
                TotalCount = cdrLogResult.TotalCount
            };
          return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, CDRLogBigResultDetailMapper);
        }

        public CDRLogDetail CDRLogDetailMapper(CDRLog cdrLog)
        {
            CarrierAccount customer=  carrierAccountManager.GetCarrierAccount(cdrLog.CustomerID);
            CarrierAccount supplier = carrierAccountManager.GetCarrierAccount(cdrLog.SupplierID);

            SaleZone salezone = saleZoneManager.GetSaleZone(cdrLog.SaleZoneID);
            SupplierZone supplierzone = supplierZoneManager.GetSupplierZone(cdrLog.SupplierZoneID);

            Switch switchEntity = switchManager.GetSwitch(cdrLog.SwitchID);
            CDRLogDetail cdrLogDetail = new CDRLogDetail
            {
                Entity = cdrLog,
                CustomerName = customer!=null?customer.Name:"N/A",
                SupplierName = supplier!=null?supplier.Name:"N/A",
                SaleZoneName = salezone != null ? salezone.Name : "N/A",
                SupplierZoneName = supplierzone != null ? supplierzone.Name : "N/A",
                SwitchName = switchEntity != null ? switchEntity.Name : "N/A",
            };
            return cdrLogDetail;
        }

    }

}

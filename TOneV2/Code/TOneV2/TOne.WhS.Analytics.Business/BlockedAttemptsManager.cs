using TOne.WhS.Analytics.Data;
using TOne.WhS.Analytics.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;
namespace TOne.WhS.Analytics.Business
{
    public class BlockedAttemptsManager
    {
        private readonly CarrierAccountManager _carrierAccountManager;
        private readonly SwitchManager _switchManager;
        private readonly SaleZoneManager _saleZoneManager;
        private readonly SupplierZoneManager _supplierZoneManager;

        public BlockedAttemptsManager()
        {
            _carrierAccountManager = new CarrierAccountManager();
            _switchManager = new SwitchManager();
            _saleZoneManager = new SaleZoneManager();
            _supplierZoneManager = new SupplierZoneManager();
        }
        public Vanrise.Entities.IDataRetrievalResult<BlockedAttemptDetail> GetBlockedAttemptData(Vanrise.Entities.DataRetrievalInput<BlockedAttemptInput> input)
        {
            IBlockedAttemptDataManager _datamanager = AnalyticsDataManagerFactory.GetDataManager<IBlockedAttemptDataManager>();
            var blockedAttemptResult = _datamanager.GetBlockedAttemptData(input);
            BigResult<BlockedAttemptDetail> blockedAttemptBigResultDetailMapper = new BigResult<BlockedAttemptDetail>
            {
                Data = blockedAttemptResult.Data.MapRecords(blockedAttemptDetailMapper),
                ResultKey = blockedAttemptResult.ResultKey,
                TotalCount = blockedAttemptResult.TotalCount
            };
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, blockedAttemptBigResultDetailMapper);
        }

        public BlockedAttemptDetail blockedAttemptDetailMapper(BlockedAttempt blockedAttempt)
        {
            CarrierAccount customer=  _carrierAccountManager.GetCarrierAccount(blockedAttempt.CustomerID);
            SaleZone salezone = _saleZoneManager.GetSaleZone(blockedAttempt.SaleZoneID);
            
            BlockedAttemptDetail blockedAttemptDetail = new BlockedAttemptDetail
            {
                Entity = blockedAttempt,
                CustomerName = customer != null ? customer.NameSuffix : "N/A",
                SaleZoneName = salezone != null ? salezone.Name : "N/A",
            };
            return blockedAttemptDetail;
        }

    }

}

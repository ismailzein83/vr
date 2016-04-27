using System.Collections.Generic;
using TOne.WhS.Analytics.Data;
using TOne.WhS.Analytics.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
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

        public Vanrise.Entities.IDataRetrievalResult<BlockedAttemptDetail> GetBlockedAttemptData(Vanrise.Entities.DataRetrievalInput<BlockedAttemptQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new BlockedAttemptRequestHandler());
        }


        public BlockedAttemptDetail BlockedAttemptDetailMapper(BlockedAttempt blockedAttempt)
        {
            BlockedAttemptDetail blockedAttemptDetail = new BlockedAttemptDetail
            {
                Entity = blockedAttempt,
                CustomerName = _carrierAccountManager.GetCarrierAccountName(blockedAttempt.CustomerID),
                SaleZoneName = _saleZoneManager.GetSaleZoneName(blockedAttempt.SaleZoneID),
            };
            return blockedAttemptDetail;
        }


        #region Private Classes

        private class BlockedAttemptRequestHandler : BigDataRequestHandler<BlockedAttemptQuery, BlockedAttempt, BlockedAttemptDetail>
        {
            public override BlockedAttemptDetail EntityDetailMapper(BlockedAttempt entity)
            {
                BlockedAttemptsManager manager = new BlockedAttemptsManager();
                return manager.BlockedAttemptDetailMapper(entity);
            }

            public override IEnumerable<BlockedAttempt> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<BlockedAttemptQuery> input)
            {
                IBlockedAttemptDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<IBlockedAttemptDataManager>();
                return dataManager.GetAllFilteredBlockedAttempts(input);
            }
        }

        #endregion

    }

}

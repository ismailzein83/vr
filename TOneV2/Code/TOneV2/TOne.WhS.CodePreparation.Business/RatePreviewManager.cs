using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.CodePreparation.Business
{
    public class RatePreviewManager
    {

        #region ctor/Local Variables
    
        SellingProductManager _sellingProductManager;
        CarrierAccountManager _carrierAccountManager;
        public RatePreviewManager()
        {
            _sellingProductManager = new SellingProductManager();
            _carrierAccountManager = new CarrierAccountManager();
        }

        #endregion


        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<RatePreviewDetail> GetFilteredRatesPreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new RatePreviewRequestHandler());
        }

        #endregion


        #region Private Classes

        private class RatePreviewRequestHandler : BigDataRequestHandler<SPLPreviewQuery, RatePreview, RatePreviewDetail>
        {
            public override RatePreviewDetail EntityDetailMapper(RatePreview entity)
            {
                RatePreviewManager manager = new RatePreviewManager();
                return manager.RatePreviewDetailMapper(entity);
            }

            public override IEnumerable<RatePreview> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
            {
                ISaleRatePreviewDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ISaleRatePreviewDataManager>();
                return dataManager.GetFilteredRatesPreview(input.Query);
            }
        }

        #endregion


        #region Private Mappers
        private RatePreviewDetail RatePreviewDetailMapper(RatePreview ratePreview)
        {
            RatePreviewDetail ratePreviewDetail = new RatePreviewDetail();
            ratePreviewDetail.Entity = ratePreview;
            ratePreviewDetail.OwnerName = GetOwnerName(ratePreview);
            ratePreviewDetail.OwnerTypeDescription = Vanrise.Common.Utilities.GetEnumDescription(ratePreview.OnwerType);
            return ratePreviewDetail;
        }

        #endregion


        #region Private Methods

        private string GetOwnerName(RatePreview ratePreview)
        {
            return ratePreview.OnwerType == BusinessEntity.Entities.SalePriceListOwnerType.SellingProduct ? _sellingProductManager.GetSellingProduct(ratePreview.OwnerId).Name
                : _carrierAccountManager.GetCarrierAccountName(ratePreview.OwnerId);
        }

        #endregion
       
    }
}

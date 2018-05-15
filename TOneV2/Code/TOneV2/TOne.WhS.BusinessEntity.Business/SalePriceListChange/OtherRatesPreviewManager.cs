using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Entities.SalePricelistChanges;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Business
{
   public class OtherRatesPreviewManager
    {
        #region ctor/Local Variables

        SellingProductManager _sellingProductManager;
        CarrierAccountManager _carrierAccountManager;
        public OtherRatesPreviewManager()
        {
            _sellingProductManager = new SellingProductManager();
            _carrierAccountManager = new CarrierAccountManager();
        }

        #endregion


        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<SalePricelistRateChangeDetail> GetFilteredRatesPreview(Vanrise.Entities.DataRetrievalInput<BaseOtherRatesPreviewQueryHandler> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new RatePreviewRequestHandler());
        }

        #endregion


        #region Private Classes

        private class RatePreviewRequestHandler : BigDataRequestHandler<BaseOtherRatesPreviewQueryHandler, SalePricelistRateChange, SalePricelistRateChangeDetail>
        {
            private SalePricelistRateChangeDetail SalePricelistRateChangeDetailMapper(SalePricelistRateChange ratePreview)
            {
                RateTypeManager rateTypeManager = new RateTypeManager();
                CurrencyManager currencyManager = new CurrencyManager();
                SalePricelistRateChangeDetail ratePreviewDetail = new SalePricelistRateChangeDetail();
                ratePreviewDetail.RateTypeId = ratePreview.RateTypeId;
                ratePreviewDetail.RecentRate = ratePreview.RecentRate;
                ratePreviewDetail.Rate = ratePreview.Rate;
                ratePreviewDetail.ChangeType = ratePreview.ChangeType;
                ratePreviewDetail.BED = ratePreview.BED;
                ratePreviewDetail.EED = ratePreview.EED;
                if (ratePreview.RateTypeId == null)
                    throw new NullReferenceException("RateTypeId");
                ratePreviewDetail.RateTypeName = rateTypeManager.GetRateTypeName(ratePreview.RateTypeId.Value);
                return ratePreviewDetail;
            }
            public override SalePricelistRateChangeDetail EntityDetailMapper(SalePricelistRateChange entity)
            {
               
                return SalePricelistRateChangeDetailMapper(entity);
            }

            public override IEnumerable<SalePricelistRateChange> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<BaseOtherRatesPreviewQueryHandler> input)
            {
                return input.Query.GetFilteredRatesPreview();
            }
        }

        #endregion


        #region Private Mappers
       

        #endregion


        #region Private Methods

        #endregion

    }
}

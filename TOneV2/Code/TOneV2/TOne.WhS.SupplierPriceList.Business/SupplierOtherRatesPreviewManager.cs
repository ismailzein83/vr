using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierOtherRatesPreviewManager
    {
        private RateTypeManager _rateTypeManager;

        public SupplierOtherRatesPreviewManager()
        {
            _rateTypeManager = new RateTypeManager();
        }
        public Vanrise.Entities.IDataRetrievalResult<OtherRatePreviewDetail> GetFilteredOtherRatesPreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SupplierOtherRatesPreviewRequestHandler());
        }


        #region Private Classes

        private class SupplierOtherRatesPreviewRequestHandler : BigDataRequestHandler<SPLPreviewQuery, OtherRatePreview, OtherRatePreviewDetail>
        {
            public override OtherRatePreviewDetail EntityDetailMapper(OtherRatePreview entity)
            {
                SupplierOtherRatesPreviewManager manager = new SupplierOtherRatesPreviewManager();
                return manager.OtherRatePreviewDetailMapper(entity);
            }

            public override IEnumerable<OtherRatePreview> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
            {
                ISupplierOtherRatePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierOtherRatePreviewDataManager>();
                return dataManager.GetFilteredOtherRatesPreview(input.Query);
            }
        }

        #endregion


        #region Private Mappers
        private OtherRatePreviewDetail OtherRatePreviewDetailMapper(OtherRatePreview otherRatePreview)
        {
            OtherRatePreviewDetail otherRatePreviewDetail = new OtherRatePreviewDetail();
            otherRatePreviewDetail.Entity = otherRatePreview;
            otherRatePreviewDetail.RateTypeDescription = _rateTypeManager.GetRateTypeName(otherRatePreview.RateTypeId);
            return otherRatePreviewDetail;
        }

        #endregion
    }
}

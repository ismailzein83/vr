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
using Vanrise.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierRatePreviewManager
    {
        public void Insert(int priceListId, IEnumerable<RatePreview> ratePreviewList)
        {
            ISupplierRatePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierRatePreviewDataManager>();
            dataManager.Insert(priceListId, ratePreviewList);
        }
        public Vanrise.Entities.IDataRetrievalResult<RatePreviewDetail> GetFilteredRatePreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            ISupplierRatePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierRatePreviewDataManager>();

            BigResult<RatePreview> ratesPreview = dataManager.GetRatePreviewFilteredFromTemp(input);
            BigResult<RatePreviewDetail> ratePreviewDetailResult = new BigResult<RatePreviewDetail>()
            {
                ResultKey = ratesPreview.ResultKey,
                TotalCount = ratesPreview.TotalCount,
                Data = ratesPreview.Data.MapRecords(RatePreviewDetailMapper)
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, ratePreviewDetailResult);
        }
        private RatePreviewDetail RatePreviewDetailMapper(RatePreview ratePreview)
        {
            RatePreviewDetail ratePreviewDetail = new RatePreviewDetail();

            ratePreviewDetail.Entity = ratePreview;
            var changeEnum = Utilities.GetEnumAttribute<RateChangeType, DescriptionAttribute>((RateChangeType)ratePreview.ChangeType);

            if (changeEnum.Description != null)
                ratePreviewDetail.ChangeTypeDecription = changeEnum.Description;
            else
                ratePreviewDetail.ChangeTypeDecription = changeEnum.ToString();
            return ratePreviewDetail;
        }
    }
}

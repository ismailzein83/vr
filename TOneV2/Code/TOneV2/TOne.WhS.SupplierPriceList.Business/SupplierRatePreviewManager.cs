using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierRatePreviewManager
    {
        public void Insert(int priceListId, IEnumerable<RatePreview> ratePreviewList)
        {
            ISupplierRatePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierRatePreviewDataManager>();
            dataManager.Insert(priceListId, ratePreviewList);
        }
        public Vanrise.Entities.IDataRetrievalResult<RatePreview> GetFilteredRatePreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            ISupplierRatePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierRatePreviewDataManager>();
            return dataManager.GetRatePreviewFilteredFromTemp(input);
        }
    }
}

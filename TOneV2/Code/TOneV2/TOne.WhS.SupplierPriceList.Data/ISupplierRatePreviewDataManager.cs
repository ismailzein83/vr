using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Data
{
    public interface ISupplierRatePreviewDataManager : IDataManager
    {
        void Insert(int priceListId, IEnumerable<RatePreview> ratePreviewList);
        Vanrise.Entities.BigResult<Entities.RatePreview> GetRatePreviewFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.SPLPreviewQuery> input);
    }
}

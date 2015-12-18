using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities.PreviewData;

namespace TOne.WhS.SupplierPriceList.Data
{
    public interface ISupplierRatePreviewDataManager : IDataManager
    {
        void Insert(int priceListId, IEnumerable<RatePreview> ratePreviewList);
    }
}

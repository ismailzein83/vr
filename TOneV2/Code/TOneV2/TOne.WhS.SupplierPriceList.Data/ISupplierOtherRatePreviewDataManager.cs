using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Data;

namespace TOne.WhS.SupplierPriceList.Data
{
    public interface ISupplierOtherRatePreviewDataManager : IDataManager, IBulkApplyDataManager<OtherRatePreview>
    {
        long ProcessInstanceId { set; }

        void ApplyPreviewOtherRatesToDB(object preparedOtherRates); 
        
        IEnumerable<OtherRatePreview> GetFilteredOtherRatesPreview(SPLPreviewQuery query);
    }
}

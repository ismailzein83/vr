using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Data;

namespace TOne.WhS.SupplierPriceList.Data
{
    public interface ISupplierRatePreviewDataManager : IDataManager, IBulkApplyDataManager<RatePreview>
    {
        long ProcessInstanceId { set; }

        void ApplyPreviewRatesToDB(object preparedRates); 

        
        Vanrise.Entities.BigResult<Entities.RatePreview> GetRatePreviewFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.SPLPreviewQuery> input);
    }
}

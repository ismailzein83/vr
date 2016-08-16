using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Data;

namespace TOne.WhS.CodePreparation.Data
{
    public interface ISaleRatePreviewDataManager : IDataManager, IBulkApplyDataManager<RatePreview>
    {
        long ProcessInstanceId { set; }

        void ApplyPreviewRatesToDB(object preparedRates);

        IEnumerable<RatePreview> GetFilteredRatesPreview(SPLPreviewQuery query);
    }
}

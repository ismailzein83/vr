using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Data;

namespace TOne.WhS.Sales.Data
{
    public interface IRatePreviewDataManager : IDataManager, IBulkApplyDataManager<RatePreview>
    {
        long ProcessInstanceId { set; }

        IEnumerable<RatePreview> GetRatePreviews(RatePreviewQuery query);

        void ApplyRatePreviewsToDB(IEnumerable<RatePreview> ratePreviews);
    }
}

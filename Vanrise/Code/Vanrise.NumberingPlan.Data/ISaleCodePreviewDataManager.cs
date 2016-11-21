using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Data
{
    public interface ISaleCodePreviewDataManager : IDataManager, IBulkApplyDataManager<CodePreview>
    {
        long ProcessInstanceId { set; }

        void ApplyPreviewCodesToDB(object preparedCodes);

        IEnumerable<CodePreview> GetFilteredCodePreview(SPLPreviewQuery query);
    }
}

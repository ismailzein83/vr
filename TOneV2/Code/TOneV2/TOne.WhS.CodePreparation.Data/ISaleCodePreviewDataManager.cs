using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Data;

namespace TOne.WhS.CodePreparation.Data
{
    public interface ISaleCodePreviewDataManager : IDataManager, IBulkApplyDataManager<CodePreview>
    {
        long ProcessInstanceId { set; }

        void ApplyPreviewCodesToDB(object preparedCodes);

        IEnumerable<CodePreview> GetFilteredCodePreview(SPLPreviewQuery query);
    }
}

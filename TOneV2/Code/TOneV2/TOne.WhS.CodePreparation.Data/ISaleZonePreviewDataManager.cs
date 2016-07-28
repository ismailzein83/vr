using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Data;

namespace TOne.WhS.CodePreparation.Data
{
    public interface ISaleZonePreviewDataManager : IDataManager, IBulkApplyDataManager<ZonePreview>
    {
        long ProcessInstanceId { set; }

        void ApplyPreviewZonesToDB(object preparedZones);

        IEnumerable<ZonePreview> GetFilteredZonePreview(SPLPreviewQuery query);
    }
}

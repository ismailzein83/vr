using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.NumberingPlan.Data;
using Vanrise.NumberingPlan.Entities;
using Vanrise.Data;

namespace Vanrise.NumberingPlan.Data
{
    public interface ISaleZonePreviewDataManager : IDataManager, IBulkApplyDataManager<ZonePreview>
    {
        long ProcessInstanceId { set; }

        void ApplyPreviewZonesToDB(object preparedZones);

        IEnumerable<ZonePreview> GetFilteredZonePreview(SPLPreviewQuery query);
    }
}

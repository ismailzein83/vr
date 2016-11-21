using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Data
{
    public interface IChangedSaleZoneDataManager : IDataManager, IBulkApplyDataManager<ChangedZone>
    {
        long ProcessInstanceId { set; }
        void ApplyChangedZonesToDB(object preparedZones);
    }
}

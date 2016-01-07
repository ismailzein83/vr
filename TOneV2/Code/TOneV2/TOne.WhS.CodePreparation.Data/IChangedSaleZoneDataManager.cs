using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.CP.Processing;
using Vanrise.Data;

namespace TOne.WhS.CodePreparation.Data
{
    public interface IChangedSaleZoneDataManager : IDataManager, IBulkApplyDataManager<ChangedZone>
    {
        long ProcessInstanceId { set; }
        void Insert(long processInstanceID, IEnumerable<ChangedZone> changedZones);

        void ApplyChangedZonesToDB(object preparedZones, long processInstanceID);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.CP.Processing;
using Vanrise.Data;

namespace TOne.WhS.CodePreparation.Data
{
    public interface INewSaleZoneDataManager : IDataManager, IBulkApplyDataManager<AddedZone>
    {
        long ProcessInstanceId { set; }

        int SellingNumberPlanId { set; }

        void Insert(int sellingNumberPlanId, long processInstanceID, IEnumerable<AddedZone> zonesList);

        void ApplyNewZonesToDB(object preparedZones, int sellingNumberPlanId, long processInstanceID);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Data
{
    public interface INewSaleZoneDataManager : IDataManager, IBulkApplyDataManager<AddedZone>
    {
        long ProcessInstanceId { set; }

        int SellingNumberPlanId { set; }

        void ApplyNewZonesToDB(object preparedZones);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Data;

namespace TOne.WhS.Sales.Data
{
    public interface IChangedSaleZoneServiceDataManager : IDataManager, IBulkApplyDataManager<ChangedSaleZoneService>
    {
        long ProcessInstanceId { set; }

        void ApplyChangedSaleZoneServicesToDB(IEnumerable<ChangedSaleZoneService> changedSaleZoneServices);
    }
}

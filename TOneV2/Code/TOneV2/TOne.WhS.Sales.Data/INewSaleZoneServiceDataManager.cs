using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Data;

namespace TOne.WhS.Sales.Data
{
    public interface INewSaleZoneServiceDataManager : IDataManager, IBulkApplyDataManager<NewSaleZoneService>
    {
        long ProcessInstanceId { set; }

        void ApplyNewSaleZoneServicesToDB(IEnumerable<NewSaleZoneService> newSaleZoneServices);
    }
}

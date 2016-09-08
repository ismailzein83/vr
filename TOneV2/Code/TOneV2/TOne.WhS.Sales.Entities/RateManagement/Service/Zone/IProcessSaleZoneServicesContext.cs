using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities.RateManagement.Service.Zone
{
    public interface IProcessSaleZoneServicesContext
    {
        IEnumerable<SaleZoneServiceToAdd> SaleZoneServicesToAdd { get; }
        IEnumerable<SaleZoneServiceToClose> SaleZoneServicesToClose { get; }
        IEnumerable<ExistingSaleZoneService> ExistingSaleZoneServices { get; }
        IEnumerable<ExistingZone> ExistingZones { get; }
        IEnumerable<NewSaleZoneService> NewSaleZoneServices { set; }
        IEnumerable<ChangedSaleZoneService> ChangedSaleZoneServices { set; }
    }
}

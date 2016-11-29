using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using TOne.WhS.Sales.Entities.RateManagement.Service.Zone;

namespace TOne.WhS.Sales.Business
{
    public class ProcessSaleZoneServicesContext : IProcessSaleZoneServicesContext
    {
        public IEnumerable<SaleZoneServiceToAdd> SaleZoneServicesToAdd { get; set; }
        public IEnumerable<SaleZoneServiceToClose> SaleZoneServicesToClose { get; set; }
        public IEnumerable<ExistingSaleZoneService> ExistingSaleZoneServices { get; set; }
        public IEnumerable<ExistingZone> ExistingZones { get; set; }
        public IEnumerable<NewSaleZoneService> NewSaleZoneServices { get; set; }
        public IEnumerable<ChangedSaleZoneService> ChangedSaleZoneServices { get; set; }
    }
}

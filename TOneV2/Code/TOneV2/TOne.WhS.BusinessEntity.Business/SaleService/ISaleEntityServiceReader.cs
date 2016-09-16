using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public interface ISaleEntityServiceReader
    {
        SaleEntityZoneServicesByZone GetSaleEntityZoneServicesByZone(SalePriceListOwnerType ownerType, int ownerId);
        SaleEntityDefaultService GetSaleEntityDefaultService(SalePriceListOwnerType ownerType, int ownerId);
    }

    public class SaleEntityZoneServicesByZone : Dictionary<long, SaleEntityZoneService>
    {

    }

    public class SaleEntityZoneServicesByOwner
    {
        public Dictionary<int, SaleEntityZoneServicesByZone> SaleEntityZoneServicesByCustomer { get; set; }
        public Dictionary<int, SaleEntityZoneServicesByZone> SaleEntityZoneServicesByProduct { get; set; }
    }


    public class SaleEntityDefaultServicesByOwner
    {
        public Dictionary<int, SaleEntityDefaultService> DefaultServicesByProduct { get; set; }
        public Dictionary<int, SaleEntityDefaultService> DefaultServicesByCustomer { get; set; }
    }
}

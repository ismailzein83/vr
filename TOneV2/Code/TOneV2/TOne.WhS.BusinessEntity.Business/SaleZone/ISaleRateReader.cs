using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public interface ISaleRateReader
    {
        SaleRatesByZone GetZoneRates(SalePriceListOwnerType ownerType, int ownerId);
    }

    public class SaleRatesByOwner
    {
        public Dictionary<int, SaleRatesByZone> SaleRatesByCustomer { get; set; }

        public Dictionary<int, SaleRatesByZone> SaleRatesByProduct { get; set; }
    }

    public class SaleRatesByZone : Dictionary<long, SaleRatePriceList>
    {

    }
}

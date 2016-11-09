using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public interface ISaleRateReader
    {
        SaleRatesByZone GetZoneRates(SalePriceListOwnerType ownerType, int ownerId);
    }

    public class SaleRatesByOwner
    {
        public VRDictionary<int, SaleRatesByZone> SaleRatesByCustomer { get; set; }

        public VRDictionary<int, SaleRatesByZone> SaleRatesByProduct { get; set; }
    }

    public class SaleRatesByZone : VRDictionary<long, SaleRatePriceList>
    {

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    /// <summary>
    /// The rates of ALL the customers assigned to the selling product are stored by rate type
    /// </summary>
    public class EffectiveAfterCustomerZoneRatesByZone : Dictionary<long, Dictionary<RateTypeKey, List<SaleRate>>>
    {

    }
}

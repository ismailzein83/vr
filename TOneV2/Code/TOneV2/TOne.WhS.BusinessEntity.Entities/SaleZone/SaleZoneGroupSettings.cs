using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class SaleZoneGroupSettings
    {
        public int ConfigId { get; set; }

        public int SellingNumberPlanId { get; set; }

        public abstract IEnumerable<long> GetZoneIds(ISaleZoneGroupContext context);

        public abstract string GetDescription(ISaleZoneGroupContext context);
    }
}

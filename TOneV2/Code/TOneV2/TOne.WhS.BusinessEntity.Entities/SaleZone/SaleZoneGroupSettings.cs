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

        public int? SellingNumberPlanId { get; set; }

        public abstract IEnumerable<long> GetZoneIds(SaleZoneGroupContext context);

        public abstract string GetDescription();
    }

    //public class SelectiveSaleZonesSettings : SaleZoneGroupSettings
    //{
    //    public List<long> ZoneIds { get; set; }

    //    public override IEnumerable<long> GetZoneIds(SaleZoneGroupContext context)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public class AllExceptSaleZonesSettings : SaleZoneGroupSettings
    //{
    //    public List<long> ZoneIds { get; set; }

    //    public override IEnumerable<long> GetZoneIds(SaleZoneGroupContext context)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}

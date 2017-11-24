using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class CodeMatches
    {
        public string CodePrefix { get; set; }

        public string Code { get; set; }

        public List<SaleCodeMatch> SaleCodeMatches { get; set; }

        public List<SupplierCodeMatchWithRate> SupplierCodeMatches { get; set; }

        public SupplierCodeMatchWithRateBySupplier SupplierCodeMatchesBySupplier { get; set; }
    }

    public class PartialCodeMatches
    {
        //public string CodePrefix { get; set; }

        public string Code { get; set; }

        public List<SupplierCodeMatchWithRate> SupplierCodeMatches { get; set; }

        public SupplierCodeMatchWithRateBySupplier SupplierCodeMatchesBySupplier { get; set; }
    }

    public class RoutingCodeMatches
    {
        //public string CodePrefix { get; set; }

        public string Code { get; set; }

        public List<SaleZoneDefintion> SaleZoneDefintions { get; set; }

        public List<SupplierCodeMatchWithRate> SupplierCodeMatches { get; set; }

        public SupplierCodeMatchWithRateBySupplier SupplierCodeMatchesBySupplier { get; set; }
    }

    public class SaleZoneDefintion
    {
        public long SaleZoneId { get; set; }

        public int SellingNumberPlanId { get; set; }
    }

    public class RPCodeMatches
    {
        public long SaleZoneId { get; set; }
        public string Code { get; set; }
        public List<SupplierCodeMatchWithRate> SupplierCodeMatches { get; set; }

    }

    public class RPCodeMatchesByZone
    {
        public long SaleZoneId { get; set; }
        public IEnumerable<SupplierCodeMatchWithRate> SupplierCodeMatches { get; set; }
        public SupplierCodeMatchesWithRateBySupplier SupplierCodeMatchesBySupplier { get; set; }

    }

    public class RPCodeMatchesByZoneBatch
    {
        public List<RPCodeMatchesByZone> RPCodeMatchesByZone { get; set; }

    }
}

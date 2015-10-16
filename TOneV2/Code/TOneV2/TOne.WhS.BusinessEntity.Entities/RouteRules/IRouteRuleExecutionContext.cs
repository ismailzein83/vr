using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public interface IRouteRuleExecutionContext
    {
        List<SupplierCodeMatch> SupplierCodeMatches { get; }

        RouteRule RouteRule { get; }
    }

    public class SupplierCodeMatch
    {
        public int SupplierId { get; set; }

        public long SupplierZoneId { get; set; }

        public string SupplierCode { get; set; }

        public Decimal SupplierRate { get; set; }
    }
}

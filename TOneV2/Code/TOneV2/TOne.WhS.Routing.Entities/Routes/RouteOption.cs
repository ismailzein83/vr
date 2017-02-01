using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RouteOption : IRouteOptionPercentageTarget
    {
        static RouteOption()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(RouteOption),
                "SupplierId", "SupplierCode", "SupplierZoneId", "SupplierRate", "Percentage", "IsBlocked", "IsFiltered", "ExecutedRuleId", "ExactSupplierServiceIds", "NumberOfTries");
        }
        public int SupplierId { get; set; }

        public string SupplierCode { get; set; }

        public long SupplierZoneId { get; set; }

        public Decimal SupplierRate { get; set; }

        public Decimal? Percentage { get; set; }

        public bool IsBlocked { get; set; }

        public bool IsFiltered { get; set; }

        public int? ExecutedRuleId { get; set; }

        public HashSet<int> ExactSupplierServiceIds { get; set; }

        public int NumberOfTries { get; set; }
    }
}

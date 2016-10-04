using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RPRouteOptionSupplierZone
    {
        static RPRouteOptionSupplierZone()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(RPRouteOptionSupplierZone),
                "SupplierCode", "SupplierZoneId", "SupplierRate", "ExactSupplierServiceIds", "ExecutedRuleId", "IsBlocked");
        }

        public string SupplierCode { get; set; }

        public long SupplierZoneId { get; set; }

        public Decimal SupplierRate { get; set; }

        public HashSet<int> ExactSupplierServiceIds { get; set; }

        public int? ExecutedRuleId { get; set; }

        public bool IsBlocked { get; set; }
    }
}

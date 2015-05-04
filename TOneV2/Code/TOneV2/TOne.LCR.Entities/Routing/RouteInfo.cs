using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities.Routing
{
    public class RouteInfo
    {
        public string CustomerID { get; set; }
        public int ZoneID { get; set; }
        public string Code { get; set; }
        public short ServiceFlag { get; set; }
        public decimal Rate { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsOverrideAffected { get; set; }
        public bool IsSpecialRequestAffected { get; set; }
        public List<RouteOptionInfo> SuppliersInfo { get; set; }
    }

    public class RouteOptionInfo
    {
        public string SupplierID { get; set; }
        public short ServiceFlag { get; set; }
        public int ZoneID { get; set; }
        public decimal SupplierRate { get; set; }
        public bool IsBlocked { get; set; }

    }

    public enum TargetType
    {
        Code,
        Zone
    }
}

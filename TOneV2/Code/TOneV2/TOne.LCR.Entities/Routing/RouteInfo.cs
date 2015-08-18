using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.LCR.Entities.Routing
{
    public class RouteInfo
    {
        public int RouteID { get; set; }
        public CarrierAccount CustomerID { get; set; }
        public Zone OurZoneID { get; set; }
        public string Code { get; set; }
        public short OurServicesFlag { get; set; }
        public float OurActiveRate { get; set; }
        public RouteState State { get; set; }
        public DateTime Updated { get; set; }
        public bool IsBlockAffected { get; set; }
        public bool IsOverrideAffected { get; set; }
        public bool IsSpecialRequestAffected { get; set; }
        public bool IsToDAffected { get; set; }
        public bool IsOptionBlock { get; set; }
        public List<RouteOptionInfo> SuppliersInfo { get; set; }
    }

    public class RouteOptionInfo
    {
        public int RouteID { get; set; }
        public CarrierAccount SupplierID { get; set; }
        public short SupplierServicesFlag { get; set; }
        public Zone SupplierZoneID { get; set; }
        public float SupplierActiveRate { get; set; }
        public byte Priority { get; set; }
        public byte NumberOfTries { get; set; }
        public byte? Percentage { get; set; }
        public RouteState State { get; set; }
    }

    public enum TargetType
    {
        Code,
        Zone
    }

    public enum RouteState : byte
    {
        Blocked = 0,
        Enabled = 1
    }
}

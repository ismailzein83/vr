using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOne.LCR.Entities.Routing;

namespace TOne.Web.Online.Models
{
    public class RouteOptionModel
    {
        //public int RouteID { get; set; }
        //public string SupplierID { get; set; }
        //public string SupplierName { get; set; }
        public short FlaggedServiceID { get; set; }
        public string Symbol { get; set; }
        public string ServiceColor { get; set; }
        //public int SupplierZoneID { get; set; }
        //public string SupplierZoneName { get; set; }
        //public float SupplierActiveRate { get; set; }
        //public byte Priority { get; set; }
        //public byte NumberOfTries { get; set; }
        //public byte? Percentage { get; set; }
        //public RouteState State { get; set; }
        public string SupplierInfoString { get; set; }
    }
}
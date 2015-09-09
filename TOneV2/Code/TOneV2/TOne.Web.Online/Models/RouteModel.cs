using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOne.LCR.Entities.Routing;

namespace TOne.Web.Online.Models
{
    public class RouteModel
    {
        public int RouteID { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public int OurZoneID { get; set; }
        public string OurZoneName { get; set; }
        public string Code { get; set; }
        public short FlaggedServiceID { get; set; }
        public string Symbol { get; set; }
        public string ServiceColor { get; set; }
        public float OurActiveRate { get; set; }
        //public RouteState State { get; set; }
        //public DateTime Updated { get; set; }
        //public bool IsBlockAffected { get; set; }
        //public bool IsOverrideAffected { get; set; }
        //public bool IsSpecialRequestAffected { get; set; }
        //public bool IsToDAffected { get; set; }
        //public bool IsOptionBlock { get; set; }
        public List<RouteOptionModel> SuppliersInfo { get; set; }
    }
}
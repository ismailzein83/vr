using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities.RouteTableRT
{
   public  class RouteTableRouteDetails
    {
       public List<RouteTableRouteDetail> Routes { get; set; }
    }

    public class RouteTableRouteDetail
    {
        public string Destination { get; set; }
        public List<RouteTableRouteOptionDetails> RouteOptionsDetails { get; set; }

    }
    public class RouteTableRouteOptionDetails
    {
        public string SupplierName { get; set; }
        public string RouteName { get; set; }
        public decimal Percentage { get; set; }
        public Int16 Preference { get; set; }
    }



}

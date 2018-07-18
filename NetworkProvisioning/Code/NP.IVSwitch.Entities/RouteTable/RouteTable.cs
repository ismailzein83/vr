using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities.RouteTable
{
    public class RouteTable
    {
        public int RouteTableId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? PScore { get; set; }
    }
    public class RouteTableInput
    {
        public RouteTable RouteTable { get; set; }
        public List<RouteTableEndPoint> EndPoints { get; set; }
    }
    public class RouteTableEndPoint
    {
        public Int16 CLIRouting { get; set; }
    }
    public class RouteTableRoute
    {
        public string Destination { get; set; }
        public List<RouteTableRouteOption> RouteOptions { get; set; }
    }
    public class RouteTableRouteOption
    {
        public int RouteId { get; set; }
        public int Percetage { get; set; }

    }

    public class RouteTableRouteDetails
    {
        public string Destination { get; set; }
        public List<RouteTableRouteOptionDetails> RouteOptions { get; set; }
    }
    public class RouteTableRouteOptionDetails
    {
        public int RouteId { get; set; }
        public string RouteName { get; set; }
        public string SupplierName { get; set; }
        public int Percetage { get; set; }

    }

}

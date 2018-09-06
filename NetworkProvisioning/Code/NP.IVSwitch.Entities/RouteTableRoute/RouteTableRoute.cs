using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.GenericData.Entities;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Entities;

namespace NP.IVSwitch.Entities.RouteTableRoute
{
    public class RouteTableRoute
    {
        public string Destination { get; set; }
        public string TechPrefix { get; set; }
        public int RouteId { get; set; }
        public List<RouteTableRouteOption> RouteOptions { get; set; }
    }
    public class RouteTableRouteOption
    {
        public int RouteId { get; set; }
        public int RoutingMode { get; set; }
        public Int16 Preference { get; set; }
        public int TotalBKTs { get; set; }
        public int BKTSerial { get; set; }
        public int BKTCapacity { get; set; }
        public int BKTTokens { get; set; }
        public decimal Percentage { get; set; }
    }


    public class RouteTableRoutesToAdd
    {
        public int RouteTableId { get; set; }
        public bool IsBlockedAccount { get; set; }
        public CodeListResolver CodeListResolver { get; set; }
        public List<RouteTableRouteOptionsToAdd> RouteOptionstoAdd { get; set; }

    }
    public class RouteTableRouteOptionsToAdd
    {
        public int RouteId { get; set; }
        public Int16 Preference { get; set; }
        public decimal? Percentage { get; set; }
    }
    public class RouteTableRoutesToEdit
    {
        public bool IsBlockedAccount { get; set; }
        public int RouteTableId { get; set; }
        public string Destination{get;set;}
        public List<RouteTableRouteOptionToEdit> RouteOptionsToEdit { get; set; }


    }
    public class RouteTableRouteOptionToEdit
    {
        public int RouteId { get; set; }
        public Int16 Preference { get; set; }
        public decimal? Percentage { get; set; }
        public int? SupplierId { get; set; }



    }


}


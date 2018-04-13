using System;
using System.Collections.Generic;

namespace TOne.WhS.RouteSync.Ericsson
{
    public interface IRouteInitializeContext
    {

    }

    public class RouteInitializeContext : IRouteInitializeContext
    {

    }

    public interface IRouteCompareTablesContext
    {
        List<EricssonConvertedRoute> RoutesToAdd { set; }
        List<EricssonConvertedRoute> RoutesToUpdate { set; }
        List<EricssonConvertedRoute> RoutesToDelete { set; }
    }

    public class RouteCompareTablesContext : IRouteCompareTablesContext
    {
        public List<EricssonConvertedRoute> RoutesToAdd { get; set; }
        public List<EricssonConvertedRoute> RoutesToUpdate { get; set; }
        public List<EricssonConvertedRoute> RoutesToDelete { get; set; }
    }

    public interface IRouteFinalizeContext
    {

    }

    public class RouteFinalizeContext : IRouteFinalizeContext
    {

    }
}
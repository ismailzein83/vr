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

    public interface ICodeGroupRouteInitializeContext
    {

    }

    public class CodeGroupRouteInitializeContext : ICodeGroupRouteInitializeContext
    {

    }


    public interface IRouteCompareTablesContext
    {
        Dictionary<int, EricssonConvertedRouteDifferences> RouteDifferencesByOBA { set; }
    }

    public class RouteCompareTablesContext : IRouteCompareTablesContext
    {
        public Dictionary<int, EricssonConvertedRouteDifferences> RouteDifferencesByOBA { get; set; }
    }

    public interface IRouteFinalizeContext
    {

    }

    public class RouteFinalizeContext : IRouteFinalizeContext
    {

    }

    public interface IRouteSucceededInitializeContext
    {

    }

    public class RouteSucceededInitializeContext : IRouteSucceededInitializeContext
    {

    }
    public interface IRouteSucceededFinalizeContext
    {

    }

    public class RouteSucceededFinalizeContext : IRouteFinalizeContext
    {

    }

    public interface INextBTableInitializeContext
    {

    }

    public class NextBTableInitializeContext : INextBTableInitializeContext
    {

    }
}
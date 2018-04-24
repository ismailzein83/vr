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
		Dictionary<string, EricssonConvertedRouteDifferences> RouteDifferencesByBO { set; }
	}

	public class RouteCompareTablesContext : IRouteCompareTablesContext
	{
		public Dictionary<string, EricssonConvertedRouteDifferences> RouteDifferencesByBO { get; set; }
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
}
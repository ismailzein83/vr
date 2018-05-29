using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Ericsson
{
	public struct EricssonConvertedRouteIdentifier
	{
		public string BO { get; set; }
		public string Code { get; set; }
		public override int GetHashCode()
		{
			return BO.GetHashCode() + Code.GetHashCode();
		}
	}
	public class EricssonConvertedRoute : ConvertedRoute
	{
		public string BO { get; set; }
		public string Code { get; set; }
		public int RCNumber { get; set; }
	}

	public class EricssonConvertedRouteCompareResult
	{
		public EricssonConvertedRoute Route { get; set; }
		public EricssonConvertedRoute OriginalValue { get; set; }
	}

	public class EricssonConvertedRouteDifferences
	{
		public EricssonConvertedRouteDifferences()
		{
			RoutesToAdd = new List<EricssonConvertedRouteCompareResult>();
			RoutesToUpdate = new List<EricssonConvertedRouteCompareResult>();
			RoutesToDelete = new List<EricssonConvertedRouteCompareResult>();
		}
		public List<EricssonConvertedRouteCompareResult> RoutesToAdd { get; set; }
		public List<EricssonConvertedRouteCompareResult> RoutesToUpdate { get; set; }
		public List<EricssonConvertedRouteCompareResult> RoutesToDelete { get; set; }
	}
	public enum EricssonConvertedRouteType
	{
		Override,
		Normal,
		Forward,
		Transit,
		Local,
		InterconnectOverride
	}
	public class ManualOverrides
	{
		public string BO { get; set; }
		public string Code { get; set; }
		public int RCNumber { get; set; }
		public string CodeGroup { get; set; }
		public EricssonConvertedRouteType Type { get; set; }
		public string IBNT { get; set; }
		public string NBNT { get; set; }
		public string IOBA { get; set; }
		public string NOBA { get; set; }
		public string M { get; set; }
		public string L { get; set; }
		public string NationalM { get; set; }
		public string CCL { get; set; }
		public string FBO { get; set; }
		public string D { get; set; }
		public string CC { get; set; }
	}
	public class InterconnectOverrides
	{
		public string BO { get; set; }
		public string Code { get; set; }
		public int RCNumber { get; set; }
		public string CodeGroup { get; set; }
		public EricssonConvertedRouteType Type { get; set; }
		public string IBNT { get; set; }
		public string NBNT { get; set; }
		public string IOBA { get; set; }
		public string NOBA { get; set; }
		public string M { get; set; }
		public string L { get; set; }
		public string NationalM { get; set; }
		public string CCL { get; set; }
		public string FBO { get; set; }
		public string D { get; set; }
		public string CC { get; set; }
		public string Prefix { get; set; }
	}
	public class EricssonRouteProperties
	{
		public string IBNT { get; set; }
		public string NBNT { get; set; }
		public string IOBA { get; set; }
		public string NOBA { get; set; }
		public string M { get; set; }
		public string CC { get; set; }
		public string CCL { get; set; }
		public string L { get; set; }
		public string D { get; set; }
		public EricssonConvertedRouteType Type { get; set; }
		public string NationalM { get; set; }
		public string FBO { get; set; }
		public bool IsOverride { get; set; }
		public bool IsInterconnectOverride { get; set; }
	}
}
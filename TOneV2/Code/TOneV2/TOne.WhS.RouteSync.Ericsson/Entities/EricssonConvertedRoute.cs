using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Ericsson
{
    public struct EricssonConvertedRouteIdentifier
    {
        public int BO { get; set; }

        public int TRD { get; set; }

        public string Code { get; set; }
        public EricssonRouteType RouteType { get; set; }
        public override int GetHashCode()
        {
            return BO.GetHashCode() + Code.GetHashCode() + RouteType.GetHashCode() + TRD.GetHashCode();
        }
    }

    public class EricssonConvertedRoute : ConvertedRouteWithCode
    {
        public int BO { get; set; }
        public int RCNumber { get; set; }
        public EricssonRouteType RouteType { get; set; }
        public int? NextBTable { get; set; }
        public int TRD { get; set; }
        public string OriginCode { get; set; }

        public override string GetCustomer()
        {
            return this.BO.ToString();
        }

        public override string GetRouteOptionsIdentifier()
        {
            return string.Format("{0}@{1}", this.RCNumber.ToString(), TRD.ToString());
        }
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

    public enum EricssonRouteType
    {
        [EricssonRouteType(IsARoute = false, IsSpecialRoute = false)]
        BNumber = 0,
        [EricssonRouteType(IsARoute = true, IsSpecialRoute = false)]
        ANumber = 1,
        [EricssonRouteType(IsARoute = false, IsSpecialRoute = true)]
        BNumberServiceLanguage = 2,
        [EricssonRouteType(IsARoute = true, IsSpecialRoute = true)]
        ANumberServiceLanguage = 3,
        [EricssonRouteType(IsARoute = false, IsSpecialRoute = false)]
        NextBTableRoute = 4
    }

    public class EricssonRouteTypeAttribute : Attribute
    {
        public bool IsSpecialRoute { get; set; }

        public bool IsARoute { get; set; }

    }

    public class NextBTableDetails
    {
        public int BO { get; set; }
        public string Prefix { get; set; }
        public int NextBTable { get; set; }
    }

}
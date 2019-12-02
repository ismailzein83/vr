using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Cataleya.Entities
{
    public interface ICataleyaFinalizeContext
    {
        Dictionary<int, FinalCustomerData> FinalCustomerDataByAccountId { get; }
        List<String> RouteTableNamesForIndexes { get; }
        Action<LogEntryType, string, object[]> WriteTrackingMessage { get; }
    }

    public class CataleyaFinalizeContext : ICataleyaFinalizeContext
    {
        public Dictionary<int, FinalCustomerData> FinalCustomerDataByAccountId { get; set; }
        public List<String> RouteTableNamesForIndexes { get; set; }
        public Action<LogEntryType, string, object[]> WriteTrackingMessage { get; set; }
    }
    public interface ICataleyaApplyDifferentialRoutesContext
    {
        Dictionary<string, List<CataleyaConvertedRoute>> RoutesByRouteTableName { get; }
        Action<LogEntryType, string, object[]> WriteTrackingMessage { get; }
    }
    public class CataleyaApplyDifferentialRoutesContext : ICataleyaApplyDifferentialRoutesContext
    {
        public Dictionary<string, List<CataleyaConvertedRoute>> RoutesByRouteTableName { get; set; }
        public Action<LogEntryType, string, object[]> WriteTrackingMessage { get; set; }
    }
}
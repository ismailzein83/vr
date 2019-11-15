using System;
using System.Collections.Generic;

namespace TOne.WhS.RouteSync.Cataleya.Entities
{
    public interface ICataleyaFinalizeContext
    {
        Dictionary<int, FinalCustomerData> FinalCustomerDataByAccountId { get; }
        List<String> RouteTableNamesForIndexes { get; }
    }
    public class CataleyaFinalizeContext : ICataleyaFinalizeContext
    {
        public Dictionary<int, FinalCustomerData> FinalCustomerDataByAccountId { get; set; }
        public List<String> RouteTableNamesForIndexes { get; set; }
    }
}
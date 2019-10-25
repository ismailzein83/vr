using System.Collections.Generic;

namespace TOne.WhS.RouteSync.Cataleya.Entities
{
    public class SupplierMapping
    {
        public List<OutTrunk> OutTrunks { get; set; }
    }

    public class OutTrunk
    {
        public string Trunk { get; set; } 

        public int? Percentage { get; set; }
    }
}
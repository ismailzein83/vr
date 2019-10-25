using System.Collections.Generic;

namespace TOne.WhS.RouteSync.Cataleya.Entities
{
    public class CustomerMapping
    {
        public List<InTrunk> InTrunks { get; set; }
    }

    public class InTrunk
    {
        public string Trunk { get; set; }
    }
}
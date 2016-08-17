
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;

namespace TOne.WhS.DBSync.Entities
{
    public class InParsedMapping
    {
        public string CustomerId { get; set; }
        public string InCarrier { get; set; }
        public StaticValues InPrefix { get; set; }
        public StaticValues InTrunk { get; set; }
    }
}

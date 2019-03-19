using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Vanrise.Reprocess.Entities
{
    public abstract class ReprocessFilter
    {

    }

    public class GenericReprocessFilter : ReprocessFilter
    {
        public Dictionary<string, List<object>> Fields { get; set; }

        public RecordQueryLogicalOperator LogicalOperator { get; set; }
    }
}
using System.Collections.Generic;

namespace Vanrise.GenericData.Entities
{
    public class CompositeRecordConditionResolvedData
    {
        public string RecordName { get; set; }

        public List<DataRecordField> Fields { get; set; }
    }
}

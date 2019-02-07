using System;
using System.Collections.Generic;

namespace Vanrise.Analytic.Entities
{
    /// <summary>
    /// Key Measure Name, Value measure value
    /// </summary>
    public class MeasureValues : Dictionary<string, MeasureValue>
    {
    }
    public class MeasureValue
    {
        public object Value { get; set; }
        public object ModifiedValue { get; set; }
        public Guid? StyleDefinitionId { get; set; }
    }
}

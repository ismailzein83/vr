using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public Guid? StyleDefinitionId { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcOutputRecord
    {
        public Guid OutputItemDefinitionId { get; set; }

        public Dictionary<string, dynamic> Records { get; set; }
    }
}

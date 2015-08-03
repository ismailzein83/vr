using System;
using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPInstanceQuery
    {
        public List<int> DefinitionsId { get; set; }

        public List<BPInstanceStatus> InstanceStatus { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
    }
}

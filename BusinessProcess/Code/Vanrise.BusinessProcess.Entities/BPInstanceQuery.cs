using System;
using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPInstanceQuery
    {
        public List<Guid> DefinitionsId { get; set; }

        public List<BPInstanceStatus> InstanceStatus { get; set; }
        public string EntityId { get; set; }
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public int Top { get; set; }
    }
}

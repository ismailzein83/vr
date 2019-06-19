using System;
using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPTaskFilter
    {
        public List<Guid> TaskTypeIds { get; set; }
        public string Title { get; set; }
    }
}

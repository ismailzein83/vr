using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediation.Generic.Entities
{
    public class MediationRecord
    {
        public long EventId { get; set; }
        public string SessionId { get; set; }
        public DateTime EventTime { get; set; }
        public EventStatus EventStatus { get; set; }
        public dynamic EventDetails { get; set; }
        public Guid MediationDefinitionId { get; set; }
    }
}

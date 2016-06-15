using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediation.Generic.Entities
{
    public class StoreStagingRecord
    {
        public int EventId { get; set; }
        public long SessionId { get; set; }
        public DateTime EventTime { get; set; }
        public EventStatus EventStatus { get; set; }
        public object EventDetails { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediation.Generic.Entities
{
    public class MultiLegSessionIdEntity
    {
        public int MediationDefinitionId { get; set; }
        public string SessionId { get; set; }
        public string LegId { get; set; }
    }
}

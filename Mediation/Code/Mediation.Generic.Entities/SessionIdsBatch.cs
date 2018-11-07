using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediation.Generic.Entities
{
    public class SessionIdsBatch
    {
        public SessionIdsBatch()
        {
            SessionIdentifiers = new Dictionary<string, SessionIdentifier>();
            LastCommittedId = null;
        }
        public Dictionary<string, SessionIdentifier> SessionIdentifiers { get; set; }
        public long? LastCommittedId { get; set; }
    }

    public class SessionIdentifier
    {
        public string SessionId { get; set; }

        public bool IsTimedOut { get; set; }
    }
}

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
            SessionIds = new HashSet<string>();
        }
        public HashSet<string> SessionIds { get; set; }
    }
}

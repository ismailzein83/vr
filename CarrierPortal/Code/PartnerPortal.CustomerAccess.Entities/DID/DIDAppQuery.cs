using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerPortal.CustomerAccess.Entities
{
    public class DIDAppQuery
    {
        public bool WithSubAccounts { get; set; }
        public Guid VRConnectionId { get; set; }
    }
}

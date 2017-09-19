using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerPortal.CustomerAccess.Entities
{
    public class AccountAppQuery
    {
        public long? ParentAccountId { get; set; }
        public Guid VRConnectionId { get; set; }
        public List<string> Columns { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerPortal.CustomerAccess.Entities
{
    public class AccountGridFieldInput
    {
        public Guid VRConnectionId { get; set; }
        public long? ParentAccountId { get; set; }
        public List<AccountGridField> AccountGridFields { get; set; }
    }
}

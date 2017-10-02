using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Entities
{
    public class EnterpriseDIDsQuery
    {
        public Guid AccountBEDefinitionId { get; set; }
        public long AccountId { get; set; }
        public Guid VRConnectionId { get; set; }
    }
}

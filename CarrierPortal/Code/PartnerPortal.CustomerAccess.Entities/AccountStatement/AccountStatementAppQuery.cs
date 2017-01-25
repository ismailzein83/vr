using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace PartnerPortal.CustomerAccess.Entities
{
    public class AccountStatementAppQuery : AccountStatementQuery
    {
        public Guid ViewId { get; set; }
    }
}

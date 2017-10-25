using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountManager.Entities
{
    public class AccountManagerQuery
    {
        public List <int> UserIds { get; set; }
        public Guid AccountManagerDefinitionId { get; set; }
    }
}

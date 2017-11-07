using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountManager.Entities
{
   public class AccountManagerAssignmentDetail
    {
       public long AccountManagerAssignementId { get; set; } 
        public Guid AccountManagerAssignementDefinitionId { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }
}

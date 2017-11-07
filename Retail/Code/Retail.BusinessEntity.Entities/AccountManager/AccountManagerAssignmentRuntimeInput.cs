using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountManagerAssignmentRuntimeInput
    {
        public long? AccountManagerAssignementId { get; set; }
        public Guid AssignmentDefinitionId { get; set; }
        public Guid AccountManagerDefinitionId { get; set; }
    }
}

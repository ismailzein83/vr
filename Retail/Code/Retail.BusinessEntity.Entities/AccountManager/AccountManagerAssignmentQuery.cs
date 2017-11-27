using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountManagerAssignmentQuery
    {
        public long? AccountManagerId { get; set; }
        public Guid AccountManagerAssignementDefinitionId { get; set; }
        public string AccountId { get; set; }
        public Guid? AccountBEDefinitionId { get; set; }
    }
}

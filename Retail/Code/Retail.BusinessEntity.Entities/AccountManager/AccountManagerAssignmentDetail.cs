using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountManagerAssignmentDetail
    {
        public long AccountManagerAssignementId { get; set; }
        public Guid AccountManagerAssignementDefinitionId { get; set; }
        public long AccountManagerId { get; set; }
        public string AccountId { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
        public string AccountName { get; set; }
        public string AccountManagerName { get; set; }
    }
}

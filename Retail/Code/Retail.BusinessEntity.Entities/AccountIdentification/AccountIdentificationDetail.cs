using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountIdentificationDetail
    {
        public long GenericRuleId { get; set; }

        public Guid GenericRuleDefinitionId { get; set; }

        public string Description { get; set; }

        public DateTime? BED { get; set; }

        public DateTime? EED { get; set; }

        public String Criteria { get; set; }
    }
}

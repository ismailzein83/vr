using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountIdentificationQuery
    {
        public Guid AccountBEDefinitionId { get; set; }

        public long AccountId { get; set; }
    }
}

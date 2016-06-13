using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountIdentification
    {
        public long AccountIdentificationId { get; set; }

        public long AccountId { get; set; }

        public int DefinitionId { get; set; }

        public AccountIdentificationSettings Settings { get; set; }
    }
}

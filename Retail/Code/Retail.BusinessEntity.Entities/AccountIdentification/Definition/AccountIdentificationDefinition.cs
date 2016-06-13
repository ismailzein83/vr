using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountIdentificationDefinition
    {
        public int AccountIdentificationDefinitionId { get; set; }

        public string Name { get; set; }

        public AccountIdentificationDefinitionSettings Settings { get; set; }
    }
}

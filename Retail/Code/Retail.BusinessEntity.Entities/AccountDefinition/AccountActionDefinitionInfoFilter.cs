using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountActionDefinitionInfoFilter
    {
        public Guid AccountBEDefinition { get; set; }
        public bool? VisibleInBalanceAlertRule { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerPortal.CustomerAccess.Entities
{
    public class CurrentAccountBalanceTile
    {
        public Vanrise.AccountBalance.Entities.CurrentAccountBalance CurrentAccountBalance { get; set; }
        public string ViewURL { get; set; }
    }
}

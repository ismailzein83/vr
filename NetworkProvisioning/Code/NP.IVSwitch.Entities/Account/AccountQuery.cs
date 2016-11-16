using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities 
{
    public class AccountQuery
    {
        public String Name { get; set; }
        public List<AccountType> AccountTypes { get; set; }

    }
}

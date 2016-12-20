using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountBalanceDetail
    {
        public AccountBalance Entity { get; set; }

        public string CurrencyDescription { get; set; }
        public AccountInfo AccountInfo { get; set; }
    }

}

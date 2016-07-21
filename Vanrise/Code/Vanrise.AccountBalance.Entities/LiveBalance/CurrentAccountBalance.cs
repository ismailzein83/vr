using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class CurrentAccountBalance
    {
        public string CurrencyDescription { get; set; }
        public decimal CurrentBalance { get; set; }
    }
}

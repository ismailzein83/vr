using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountStatementResult : BigResult<AccountStatementItem>
    {
        public decimal CurrentBalance { get; set; }
        public string Currency { get; set; }
    }
}

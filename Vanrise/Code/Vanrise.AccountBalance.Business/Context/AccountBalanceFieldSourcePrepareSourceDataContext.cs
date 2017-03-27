using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class AccountBalanceFieldSourcePrepareSourceDataContext : IAccountBalanceFieldSourcePrepareSourceDataContext
    {
        public IEnumerable<Entities.AccountBalance> AccountBalances { get; set; }

        public Guid AccountTypeId { get; set; }

        public AccountTypeSettings AccountTypeSettings { get; set; }
    }
}

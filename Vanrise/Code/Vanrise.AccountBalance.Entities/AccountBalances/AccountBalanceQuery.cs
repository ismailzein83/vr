using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountBalanceQuery
    {
        public List<String> AccountsIds { get; set; }
        public Guid AccountTypeId { get; set; }
        public int Top { get; set; }

        public string Sign { get; set; }

        public decimal Balance { get; set; }

        public string OrderBy { get; set; }
        public Vanrise.Entities.VRAccountStatus? Status { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool? IsEffectiveInFuture { get; set; }

    }
}

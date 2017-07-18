using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountStatementQuery
    {
        public String AccountId { get; set; }
        public Guid AccountTypeId { get; set; }
        public DateTime FromDate { get; set; }
        public Vanrise.Entities.VRAccountStatus? Status { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool? IsEffectiveInFuture { get; set; }
    }
}

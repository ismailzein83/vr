using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.InvToAccBalanceRelation.Entities;

namespace Vanrise.InvToAccBalanceRelation.Business
{
    public class InvToAccBalanceRelGetBalanceInvoiceAccountsContext : IInvToAccBalanceRelGetBalanceInvoiceAccountsContext
    {
        public Guid AccountTypeId { get; set; }

        public string AccountId { get; set; }

        public DateTime EffectiveOn { get; set; }
    }
}

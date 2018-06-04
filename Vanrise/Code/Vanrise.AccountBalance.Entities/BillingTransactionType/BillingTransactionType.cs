using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class BillingTransactionType
    {
        public Guid BillingTransactionTypeId { get; set; }
        public string Name { get; set; }
        public bool IsCredit { get; set; }
        public BillingTransactionTypeSettings Settings { get; set; }
    }

    public class BillingTransactionTypeSettings
    {
        /// <summary>
        /// true if the user is not allowed to manually add a transaction of this type
        /// </summary>
        public bool ManualAdditionDisabled { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Business;

namespace TOne.WhS.AccountBalance.MainExtensions.FinancialAccountTypes.SupplierPostpaid
{
    public class SupplierPostpaidDefinitionSettings : AccountBalanceSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("273B1F9B-FC8B-4BB5-B473-DFA637CDF290"); }
        }

        public override bool IsApplicableToCustomer
        {
            get
            {
                return false;
            }
        }

        public override bool IsApplicableToSupplier
        {
            get
            {
                return true;
            }
        }

        public Guid UsageTransactionTypeId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Business;

namespace TOne.WhS.AccountBalance.MainExtensions.FinancialAccountTypes.SupplierPrepaid
{
    public class SupplierPrepaidDefinitionSettings : AccountBalanceSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("A892CBF2-AF5F-41B2-959F-742723D3B856"); }
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

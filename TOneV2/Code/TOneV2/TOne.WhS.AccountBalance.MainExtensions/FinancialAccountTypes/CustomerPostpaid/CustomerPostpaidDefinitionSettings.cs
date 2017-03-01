using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Business;

namespace TOne.WhS.AccountBalance.MainExtensions.FinancialAccountTypes.CustomerPostpaid
{
    public class CustomerPostpaidDefinitionSettings : AccountBalanceSettings
    {
        public override Guid ConfigId { get { return new Guid("86C0AA84-3477-4206-B2BA-8B104CEFEF5C"); } }

        public override bool IsApplicableToCustomer { get { return true; } }

        public override bool IsApplicableToSupplier { get { return false; } }

        public override string RuntimeEditor { get { return "whs-accountbalance-runtime-customerpostpaid"; } }

        public Guid UsageTransactionTypeId { get; set; }


        public override List<Guid> GetUsageTransactionTypes(IGetUsageTransactionTypesContext context)
        {
            return new List<Guid>() { this.UsageTransactionTypeId };
        }
    }
}

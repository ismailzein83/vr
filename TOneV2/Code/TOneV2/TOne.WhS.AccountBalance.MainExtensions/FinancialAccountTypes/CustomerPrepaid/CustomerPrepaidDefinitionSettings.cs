using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Business;
using TOne.WhS.AccountBalance.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions.FinancialAccountTypes.CustomerPrepaid
{
    public class CustomerPrepaidDefinitionSettings : AccountBalanceSettings
    {
        public override Guid ConfigId { get { return new Guid("9287977D-880C-42BA-AA45-46665F8B4546"); } }

        public override bool IsApplicableToCustomer { get { return true; } }

        public override bool IsApplicableToSupplier { get { return false; } }

        public override string RuntimeEditor { get { return "whs-accountbalance-runtime-customerprepaid"; } }

        public Guid UsageTransactionTypeId { get; set; }


        public override List<Guid> GetUsageTransactionTypes(IGetUsageTransactionTypesContext context)
        {
            return new List<Guid>() { this.UsageTransactionTypeId };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Business;

namespace TOne.WhS.AccountBalance.MainExtensions.FinancialAccountTypes.Netting
{
    public class NettingDefinitionSettings : AccountBalanceSettings
    {
        public override Guid ConfigId { get { return new Guid("097D526A-EE27-4DF4-BD45-B847D24CAA12"); } }

        public override bool IsApplicableToCustomer { get { return true; } }

        public override bool IsApplicableToSupplier { get { return true; } }

        public override string RuntimeEditor { get { return "whs-accountbalance-runtime-netting"; } }

        public Guid CustomerUsageTransactionTypeId { get; set; }

        public Guid SupplierUsageTransactionTypeId { get; set; }


        public override List<Guid> GetUsageTransactionTypes(IGetUsageTransactionTypesContext context)
        {
            return new List<Guid>() { this.CustomerUsageTransactionTypeId, this.SupplierUsageTransactionTypeId };
        }
    }
}

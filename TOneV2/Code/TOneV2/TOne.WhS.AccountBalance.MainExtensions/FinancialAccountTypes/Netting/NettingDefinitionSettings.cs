using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions.FinancialAccountTypes.Netting
{
    public class NettingDefinitionSettings : FinancialAccountDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsApplicableToCustomer
        {
            get
            {
                return true;
            }
        }

        public override bool IsApplicableToSupplier
        {
            get
            {
                return true;
            }
        }

        public Guid AccountTypeId { get; set; }

        public Guid CustomerUsageTransactionTypeId { get; set; }

        public Guid SupplierUsageTransactionTypeId { get; set; }
    }
}

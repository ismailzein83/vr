using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions.FinancialAccountTypes.CustomerPrepaid
{
    public class CustomerPrepaidDefinitionSettings : FinancialAccountDefinitionExtendedSettings
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
            get { return false; }
        }

        public Guid AccountTypeId { get; set; }

        public Guid UsageTransactionTypeId { get; set; }
    }
}

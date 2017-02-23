using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions.FinancialAccountTypes.CustomerPostpaid
{
    public class CustomerPostpaidDefinitionSettings : FinancialAccountDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("5A65D5B0-0D85-40C3-B51A-3F6CE2E08E64"); }
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.FinancialAccountTypes.CustomerPostpaid
{
    public class CustomerPostpaidDefinitionSettings : WHSFinancialAccountDefinitionExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("CEB92F56-4A1E-4C06-AEFF-4EF8CF69FECF"); } }

        public override bool IsApplicableToCustomer { get { return true; } }

        public override bool IsApplicableToSupplier { get { return false; } }

        public override string RuntimeEditor { get { return "whs-be-financialaccountruntimesettings-customerpostpaid"; } }
    }
}
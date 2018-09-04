using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.FinancialAccountTypes.CustomerPrepaid
{
    public class CustomerPrepaidDefinitionSettings : WHSFinancialAccountDefinitionExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("D36D9E9B-FEB9-4A1C-AAE4-73F8296E26FF"); } }

        public override bool IsApplicableToCustomer { get { return true; } }

        public override bool IsApplicableToSupplier { get { return false; } }

        public override string RuntimeEditor { get { return "whs-be-financialaccountruntimesettings-customerprepaid"; } }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.FinancialAccountTypes.SupplierPrepaid
{
    public class SupplierPrepaidDefinitionSettings : WHSFinancialAccountDefinitionExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("23FC7BF0-111C-4B01-81F7-CA00D6073D50"); } }

        public override bool IsApplicableToCustomer { get { return false; } }

        public override bool IsApplicableToSupplier { get { return true; } }

        public override string RuntimeEditor { get { return "whs-be-financialaccountruntimesettings-supplierprepaid"; } }
    }
}
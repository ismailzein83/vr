using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.FinancialAccountTypes.Netting
{
    public class NettingDefinitionSettings : WHSFinancialAccountDefinitionExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("9D509D1B-5473-464E-BABB-FCD5D7F4701B"); } }

        public override bool IsApplicableToCustomer { get { return true; } }

        public override bool IsApplicableToSupplier { get { return true; } }

        public override string RuntimeEditor { get { return "whs-be-financialaccountruntimesettings-netting"; } }
    }
}
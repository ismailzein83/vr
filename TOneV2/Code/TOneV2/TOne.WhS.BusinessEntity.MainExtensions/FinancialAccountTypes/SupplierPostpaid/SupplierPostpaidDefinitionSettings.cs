using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.FinancialAccountTypes.SupplierPostpaid
{
    public class SupplierPostpaidDefinitionSettings : WHSFinancialAccountDefinitionExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("068258C1-0F26-441A-A219-1783741B28F2"); } }

        public override bool IsApplicableToCustomer { get { return false; } }

        public override bool IsApplicableToSupplier { get { return true; } }

        public override string RuntimeEditor { get { return "whs-be-financialaccountruntimesettings-supplierpostpaid"; } }
    }
}
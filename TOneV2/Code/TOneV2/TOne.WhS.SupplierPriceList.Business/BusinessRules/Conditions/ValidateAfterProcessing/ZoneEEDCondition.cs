using System;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    class ZoneEEDCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target as ExistingZone != null;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            IImportSPLContext importSplContext = context.GetExtension<IImportSPLContext>();

            ExistingZone existingZone = context.Target as ExistingZone;
            return (existingZone.ChangedZone == null || existingZone.ChangedZone.EED >= DateTime.Now.Add(importSplContext.CodeCloseDateOffset));
        }
        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has been closed in a period less than system period", (target as ExistingZone).Name);
        }
    }
}

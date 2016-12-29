using System;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.Business
{
    class ZoneEEDCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target as NotImportedZone != null;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            NotImportedZone notImportedZone = context.Target as NotImportedZone;

            if (!notImportedZone.HasChanged)
                return true;

            IImportSPLContext importSplContext = context.GetExtension<IImportSPLContext>();
            var result = !notImportedZone.EED.VRLessThanOrEqual(DateTime.Today.Add(importSplContext.CodeCloseDateOffset));

            if (result == false)
                context.Message = string.Format("Zone {0} has been closed in a period less than system period", notImportedZone.ZoneName);

            return result;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has been closed in a period less than system period", (target as NotImportedZone).ZoneName);
        }
    }
}

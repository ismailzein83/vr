using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class EmptyPriceListCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as AllImportedDataByZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            //AllImportedDataByZone allImportedDataByZone = context.Target as AllImportedDataByZone;
            //var invalidZones = new HashSet<string>();

            //if (allImportedDataByZone.ImportedDataByZoneList.Count() == 0)
            //{
            //    context.Message = string.Format("The imported pricelist is empty");
            //    return false;
            //}

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}

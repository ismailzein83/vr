using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class MissingZonesCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as AllImportedCodes != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            AllImportedCodes allImportedCodes = context.Target as AllImportedCodes;
            var invalidCodes = new HashSet<string>();
            int invalidRecordsCounter = 0;
            foreach (var importedCode in allImportedCodes.ImportedCodes)
            {
                if (string.IsNullOrEmpty(importedCode.ZoneName))
                {
                    if (string.IsNullOrEmpty(importedCode.Code))
                        invalidRecordsCounter++;
                    else invalidCodes.Add(importedCode.Code);
                }
            }
            if (invalidCodes.Count > 0)
                context.Message = string.Format("Zone is missing for the following code(s): ({0}).", string.Join(", ", invalidCodes));
            if(invalidRecordsCounter>0)
                context.Message += string.Format(" {0} record(s) have missing zone and code.", string.Join(", ", invalidRecordsCounter));
            return (invalidRecordsCounter == 0 && invalidCodes.Count == 0);

        }

        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }

    }
}

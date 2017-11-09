﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class CodeBEDLessThanRetroactiveDateCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is AllImportedDataByZone;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            var allData = context.Target as AllImportedDataByZone;

            if (allData.ImportedDataByZoneList == null || allData.ImportedDataByZoneList.Count() == 0)
                return true;

            var invalidCodes = new HashSet<string>();
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();

            foreach (ImportedDataByZone zoneData in allData.ImportedDataByZoneList)
            {
                foreach (var importedCode in zoneData.ImportedCodes)
                {
                    if (importedCode.BED < importSPLContext.RetroactiveDate)
                    {
                        invalidCodes.Add(importedCode.Code);
                        break;
                    }
                }
            }

            if (invalidCodes.Count > 0)
            {
                string retroactiveDateString = importSPLContext.RetroactiveDate.ToString(importSPLContext.DateFormat);
                context.Message = string.Format("Adding codes with BED less than the retroactive date '{0}'. Following are violated codes: ({1}).", retroactiveDateString, string.Join(", ", invalidCodes));
                return false;
            }

            return true;
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}

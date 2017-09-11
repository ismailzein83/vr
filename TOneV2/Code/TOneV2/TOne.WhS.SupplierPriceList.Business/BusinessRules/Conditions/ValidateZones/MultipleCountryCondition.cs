﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class MultipleCountryCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedDataByZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedDataByZone zone = context.Target as ImportedDataByZone;

            var firstCode = zone.ImportedCodes.FirstOrDefault();
            if (firstCode != null)
            {
                int? firstCodeCountryId = firstCode.CodeGroup != null ? firstCode.CodeGroup.CountryId : (int?)null;

                foreach (ImportedCode importedCode in zone.ImportedCodes)
                {
                    if (importedCode.CodeGroup != null && firstCodeCountryId.HasValue && importedCode.CodeGroup.CountryId != firstCodeCountryId.Value)
                    {
                        CountryManager countryManager = new CountryManager();
                        string firstCountryName = countryManager.GetCountryName(firstCodeCountryId.Value);
                        string importedCodeCountryName = countryManager.GetCountryName(importedCode.CodeGroup.CountryId);

                        context.Message = string.Format("Can not add Zone {0} because it has the codes {1}, {2} belongs to different countries {3} and {4}", zone.ZoneName, firstCode.Code, importedCode.Code, firstCountryName, importedCodeCountryName);
                        return false;
                    }
                }
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has multiple codes that belong to different countries", (target as ImportedDataByZone).ZoneName);
        }

    }
}

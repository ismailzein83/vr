﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class MultipleCountryRule : BusinessRule
    {
        public override bool Validate(IRuleTarget target)
        {
            ImportedZone zone = target as ImportedZone;

            bool result = true;

            if(zone.ImportedCodes != null)
            {
                var firstCode = zone.ImportedCodes.FirstOrDefault();
                if (firstCode != null)
                {
                    int? firstCodeCountryId = firstCode.CodeGroup != null ? firstCode.CodeGroup.CountryId : (int?)null;
                    Func<ImportedCode, bool> pred = new Func<ImportedCode, bool>((code) => code.CodeGroup != null && code.CodeGroup.CountryId != firstCodeCountryId.Value);
                    result = !zone.ImportedCodes.Any(pred);
                }
            }

            return result;
        }
    }
}

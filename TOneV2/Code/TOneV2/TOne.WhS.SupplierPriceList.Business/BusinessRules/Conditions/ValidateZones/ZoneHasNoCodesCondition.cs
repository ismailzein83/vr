﻿using System;
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
    public class ZoneHasNoCodesCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedZone importedZone = context.Target as ImportedZone;
            return !(importedZone.ImportedCodes.Count() == 0);
        }

        public override string GetMessage(IRuleTarget target)
        {
            ImportedZone importedZone = target as ImportedZone;
            string zoneName = importedZone.ImportedRates.First().ZoneName;
            return string.Format("Zone {0} has no codes", zoneName);
        }

    }
}

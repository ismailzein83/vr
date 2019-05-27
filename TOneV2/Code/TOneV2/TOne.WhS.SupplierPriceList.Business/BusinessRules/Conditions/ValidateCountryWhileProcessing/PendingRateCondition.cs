using System;
using Vanrise.Common;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    class PendingRateCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is AllImportedCountries;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            return true;
        }
        private Dictionary<string, List<SupplierRate>> StructureSupplierRateByZoneName(IEnumerable<SupplierRate> supplierRates)
        {
            var supplierZoneManager = new SupplierZoneManager();
            var supplierRateByZoneId = new Dictionary<string, List<SupplierRate>>();
            foreach (var supplierRate in supplierRates)
            {
                string supplierZoneName = supplierZoneManager.GetSupplierZoneName(supplierRate.ZoneId);
                List<SupplierRate> rates = supplierRateByZoneId.GetOrCreateItem(supplierZoneName);
                rates.Add(supplierRate);
            }
            return supplierRateByZoneId;
        }
        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }

    }
}

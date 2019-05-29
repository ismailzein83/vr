using System;
using System.Linq;
using Vanrise.Common;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
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
            var allImportedCountries = context.Target as AllImportedCountries;
            var zonewWithCancelPendingRates = new HashSet<string>();

            foreach (var importedCountry in allImportedCountries.ImportedCountries)
            {
                foreach (var importedZone in importedCountry.ImportedZones)
                {
                    var importedRate = importedZone.ImportedNormalRate;
                    importedRate.ThrowIfNull("ImportedRate", importedZone.ZoneName);

                    if (HasCanceledPendingRate(importedRate.ChangedExistingRates))
                        zonewWithCancelPendingRates.Add(importedRate.ZoneName);

                    foreach (var importedOtherRate in importedZone.ImportedOtherRates.Values)
                    {
                        if (HasCanceledPendingRate(importedOtherRate.ChangedExistingRates))
                            zonewWithCancelPendingRates.Add(importedOtherRate.ZoneName);
                    }
                }
            }

            if (zonewWithCancelPendingRates.Any())
            {
                context.Message = string.Format("Following zone(s) have cancel pending rates :'{0}'", string.Join("; ", zonewWithCancelPendingRates));
                return false;
            }
            return true;
        }
        private bool HasCanceledPendingRate(List<ExistingRate> ChangedExistingRates)
        {
            if (ChangedExistingRates == null || !ChangedExistingRates.Any())
                return false;

            foreach (var changedExistingRate in ChangedExistingRates)
            {
                if (changedExistingRate.BED > DateTime.Today
                    && changedExistingRate.EED.HasValue
                    && changedExistingRate.BED == changedExistingRate.EED)
                    return true;
            }
            return false;
        }
        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }

    }
}

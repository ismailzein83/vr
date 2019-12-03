using System;
using System.Linq;
using Vanrise.Common;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public sealed class AdjustImportedRateCodeEED : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ImportedZone>> ImportedZones { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ImportedZone> importedZones = ImportedZones.Get(context);

            foreach (var importedZone in importedZones)
            {
                importedZone.ImportedNormalRate.ThrowIfNull(" importedZone.ImportedNormalRate");
                importedZone.ImportedCodes.ThrowIfNull(" importedZone.ImportedCodes");

                if (importedZone.ImportedCodes.Count == 0)
                    throw new VRBusinessException("Empty imported codes");

                DateTime? rateEED = importedZone.ImportedNormalRate.EED;

                bool allCodesHasEED = true;
                DateTime? maxCodeEED = null;
                foreach (var importedCode in importedZone.ImportedCodes)
                {
                    if (!importedCode.EED.HasValue)
                        allCodesHasEED = false;
                    else
                    {
                        maxCodeEED = importedCode.EED.Value > maxCodeEED
                                     ? importedCode.EED.Value
                                     : maxCodeEED;
                    }
                }

                if (!rateEED.HasValue && !maxCodeEED.HasValue)
                    continue;

                if (rateEED.HasValue)
                {
                    foreach (var importedCode in importedZone.ImportedCodes)
                    {
                        importedCode.EED =   importedCode.EED.HasValue
                                           ? (importedCode.EED.Value > rateEED.Value ? importedCode.EED.Value : rateEED.Value)
                                           : rateEED;
                    }
                }
                if (allCodesHasEED && maxCodeEED.HasValue && !rateEED.HasValue)
                {
                    importedZone.ImportedNormalRate.EED = maxCodeEED;
                }
            }
        }
    }
}

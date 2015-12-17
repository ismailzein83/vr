using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class StructureDataByZones : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ImportedCode>> ImportedCodes { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedRate>> ImportedRates { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ImportedZone>> ImportedZones { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ImportedCode> importedCodesList = this.ImportedCodes.Get(context);
            IEnumerable<ImportedRate> importedRatesList = this.ImportedRates.Get(context);

            Dictionary<string, ImportedZone> importedZonesByZoneName = new Dictionary<string, ImportedZone>();
            ImportedZone importedZone;

            foreach (ImportedCode code in importedCodesList)
            {
                if(!importedZonesByZoneName.TryGetValue(code.ZoneName, out importedZone))
                {
                    importedZone = new ImportedZone();
                    importedZone.ImportedCodes = new List<ImportedCode>();
                    importedZone.ImportedRates = new List<ImportedRate>();
                    importedZonesByZoneName.Add(code.ZoneName, importedZone);
                }

                importedZone.ImportedCodes.Add(code);
            }

            foreach (ImportedRate rate in importedRatesList)
            {
                if (!importedZonesByZoneName.TryGetValue(rate.ZoneName, out importedZone))
                {
                    importedZone.ImportedRates = new List<ImportedRate>();
                    importedZonesByZoneName.Add(rate.ZoneName, importedZone);
                }

                importedZone.ImportedRates.Add(rate);
            }

            this.ImportedZones.Set(context, importedZonesByZoneName.Values);
        }
    }
}

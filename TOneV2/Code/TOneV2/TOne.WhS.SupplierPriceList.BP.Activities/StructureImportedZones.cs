using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Common;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class StructureImportedZones : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ImportedDataByZone>> ImportedDataByZone { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ImportedZone>> ImportedZones { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ImportedDataByZone> importedDataByZones = ImportedDataByZone.Get(context);

            List<ImportedZone> importedZones = new List<ImportedZone>();
            foreach (ImportedDataByZone importedDataByZone in importedDataByZones)
            {
                ImportedZone importedZone = new ImportedZone();
                importedZone.ZoneName = importedDataByZone.ZoneName;

                importedZone.ImportedCodes.AddRange(importedDataByZone.ImportedCodes);

                importedZone.ImportedNormalRate = importedDataByZone.ImportedNormalRates.First();

                foreach (KeyValuePair<int, List<ImportedRate>> kvp in importedDataByZone.ImportedOtherRates)
                {
                    importedZone.ImportedOtherRates.Add(kvp.Key, kvp.Value.First());
                }
               
                if(importedDataByZone.ImportedZoneServicesToValidate.Count > 0)
                {
                    importedZone.ImportedZoneServiceGroup = new ImportedZoneServiceGroup()
                    {
                        ServiceIds = importedDataByZone.ImportedZoneServicesToValidate.Keys.ToList(),
                        ZoneName = importedDataByZone.ZoneName,
                        BED = importedDataByZone.ImportedZoneServicesToValidate.Values.First().First().BED
                    };
                }

                importedZones.Add(importedZone);
            }

            this.ImportedZones.Set(context, importedZones);
        }
    }
}

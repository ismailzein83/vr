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
            IEnumerable<ImportedDataByZone> importedDataByZone = ImportedDataByZone.Get(context);
           
            List<ImportedZone> importedZones = new List<ImportedZone>();
            foreach (ImportedDataByZone item in importedDataByZone)
            {
                ImportedZone importedZone = new ImportedZone();
                importedZone.ZoneName = item.ZoneName;
                
                importedZone.ImportedCodes.AddRange(item.ImportedCodes);
               
                importedZone.ImportedNormalRate = item.ImportedRates.First(itm => !itm.RateTypeId.HasValue);
                
                IEnumerable<ImportedRate> importedOtherRates = item.ImportedRates.FindAllRecords(itm => itm.RateTypeId.HasValue);

                foreach (ImportedRate importedRate in importedOtherRates)
                {
                    ImportedRate matchImportedRate;
                    if (!importedZone.ImportedOtherRates.TryGetValue(importedRate.RateTypeId.Value, out matchImportedRate))
                        importedZone.ImportedOtherRates.Add(importedRate.RateTypeId.Value, importedRate);
                }

                importedZone.ImportedZoneService = item.ImportedZonesServices.FirstOrDefault();

                importedZones.Add(importedZone);
            }

            this.ImportedZones.Set(context, importedZones);
        }
    }
}

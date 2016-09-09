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
               
                importedZone.ImportedNormalRate = item.ImportedNormalRates.First();
                
                foreach (KeyValuePair<int, List<ImportedRate>> kvp in item.ImportedOtherRates)
                {
                    importedZone.ImportedOtherRates.Add(kvp.Key, kvp.Value.First());
                }
               
                ImportedZoneService importedZoneService = item.ImportedZonesServices.FirstOrDefault(itm => itm.BED != DateTime.MinValue);
                if (importedZoneService != null)
                {
                    importedZoneService.ServiceIds = item.ImportedZonesServices.SelectMany(itm => itm.ServiceIds).ToList();
                    importedZone.ImportedZoneService = importedZoneService;
                }

                importedZones.Add(importedZone);
            }

            this.ImportedZones.Set(context, importedZones);
        }
    }
}

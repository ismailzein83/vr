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
        public OutArgument<IEnumerable<ImportedDataByZone>> ImportedDataByZone { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ImportedCode> importedCodesList = this.ImportedCodes.Get(context);
            IEnumerable<ImportedRate> importedRatesList = this.ImportedRates.Get(context);

            Dictionary<string, ImportedDataByZone> importedDataByZoneName = new Dictionary<string, ImportedDataByZone>(StringComparer.InvariantCultureIgnoreCase);
            ImportedDataByZone importedDataByZone;

            foreach (ImportedCode code in importedCodesList)
            {

                if (!importedDataByZoneName.TryGetValue(code.ZoneName, out importedDataByZone))
                {
                    importedDataByZone = new ImportedDataByZone();
                    importedDataByZone.ZoneName = code.ZoneName;
                    importedDataByZoneName.Add(code.ZoneName, importedDataByZone);
                }

                importedDataByZone.ImportedCodes.Add(code);
            }

            foreach (ImportedRate rate in importedRatesList)
            {
                if (!importedDataByZoneName.TryGetValue(rate.ZoneName, out importedDataByZone))
                {
                    //This case will happen if a zone only exists in imported rates list
                    //adding it to the dictionary is for validation purpose (business rule)
                    if (importedDataByZone == null)
                        importedDataByZone = new ImportedDataByZone();

                    importedDataByZoneName.Add(rate.ZoneName, importedDataByZone);
                }

                importedDataByZone.ImportedRates.Add(rate);
            }

            this.ImportedDataByZone.Set(context, importedDataByZoneName.Values);
        }
    }
}

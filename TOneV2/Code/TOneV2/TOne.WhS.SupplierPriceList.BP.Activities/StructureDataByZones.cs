using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class StructureDataByZones : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ImportedCode>> ImportedCodes { get; set; }
        
        [RequiredArgument]
        public InArgument<IEnumerable<PriceListCode>> FilteredImportedCodes { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedRate>> ImportedRates { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedZoneService>> ImportedZonesServices { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ImportedDataByZone>> ImportedDataByZone { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ImportedCode> importedCodesList = this.ImportedCodes.Get(context);
            IEnumerable<PriceListCode> filteredImportedCodesList = this.FilteredImportedCodes.Get(context);
            IEnumerable<ImportedRate> importedRatesList = this.ImportedRates.Get(context);
            IEnumerable<ImportedZoneService> importedZonesServices = this.ImportedZonesServices.Get(context);

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
                    if (!filteredImportedCodesList.Any(x => x.ZoneName == rate.ZoneName))
                    {
                        //This case will happen if a zone only exists in imported rates list
                        //adding it to the dictionary is for validation purpose (business rule)
                        importedDataByZone = new ImportedDataByZone();
                        importedDataByZone.ZoneName = rate.ZoneName;

                        importedDataByZoneName.Add(rate.ZoneName, importedDataByZone);
                    }
                }
                else
                {
                    if (rate.RateTypeId == null)
                    {
                        importedDataByZone.ImportedNormalRates.Add(rate);
                    }
                    else
                    {
                        List<ImportedRate> otherRates = null;
                        if (!importedDataByZone.ImportedOtherRates.TryGetValue(rate.RateTypeId.Value, out otherRates))
                        {
                            otherRates = new List<ImportedRate>();
                            importedDataByZone.ImportedOtherRates.Add(rate.RateTypeId.Value, otherRates);
                        }

                        otherRates.Add(rate);
                    }
                }
            }

            List<ImportedZoneService> importedZoneServices;

            foreach (ImportedZoneService service in importedZonesServices)
            {
                if (!importedDataByZoneName.TryGetValue(service.ZoneName, out importedDataByZone))
                {
                    if (!filteredImportedCodesList.Any(x => x.ZoneName == service.ZoneName))
                    {
                        importedDataByZone = new ImportedDataByZone();
                        importedDataByZone.ZoneName = service.ZoneName;
                        importedZoneServices = new List<ImportedZoneService>();
                        importedZoneServices.Add(service);
                        importedDataByZone.ImportedZoneServicesToValidate.Add(service.ServiceId, importedZoneServices);
                        importedDataByZoneName.Add(service.ZoneName, importedDataByZone);
                    }
                }
                else
                {
                    if (!importedDataByZone.ImportedZoneServicesToValidate.TryGetValue(service.ServiceId, out importedZoneServices))
                    {
                        importedZoneServices = new List<ImportedZoneService>();
                        importedZoneServices.Add(service);
                        importedDataByZone.ImportedZoneServicesToValidate.Add(service.ServiceId, importedZoneServices);
                    }
                    else
                        importedZoneServices.Add(service);
                }
            }

            this.ImportedDataByZone.Set(context, importedDataByZoneName.Values);
        }
    }
}

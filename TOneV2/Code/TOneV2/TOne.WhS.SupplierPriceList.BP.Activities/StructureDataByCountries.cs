using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Common.Business;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class StructureDataByCountries : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ImportedZone>> ImportedZones { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ImportedCountry>> ImportedCountries { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ImportedZone> importedZonesList = this.ImportedZones.Get(context);

            Dictionary<int, ImportedCountry> importedCountriesByCountryId = new Dictionary<int, ImportedCountry>();
            ImportedCountry importedCountry;

            foreach (ImportedZone zone in importedZonesList)
            {
                
                    ImportedCode includedCodeOneMatch = zone.ImportedCodes.FirstOrDefault();
                    if (includedCodeOneMatch == null)
                        continue;

                    int countryId = includedCodeOneMatch.CodeGroup.CountryId;

                    if(!importedCountriesByCountryId.TryGetValue(countryId, out importedCountry))
                    {
                        importedCountry = new ImportedCountry();
                        importedCountry.CountryId = countryId;
                        importedCountry.ImportedZones = new List<ImportedZone>();
                        importedCountry.ImportedCodes = new List<ImportedCode>();
                        importedCountry.ImportedRates = new List<ImportedRate>();
                        importedCountriesByCountryId.Add(countryId, importedCountry);
                    }

                    foreach (ImportedCode code in zone.ImportedCodes)
                        importedCountry.ImportedCodes.Add(code);

                    foreach (ImportedRate rate in zone.ImportedRates)
                        importedCountry.ImportedRates.Add(rate);

                    importedCountry.ImportedZones.Add(zone);

            }

            this.ImportedCountries.Set(context, importedCountriesByCountryId.Values);
        }
    }
}

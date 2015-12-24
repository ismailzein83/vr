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
                if(!zone.IsExcluded)
                {
                    ImportedCode includedCodeOneMatch = zone.ImportedCodes.Find(x => !x.IsExcluded);
                    if (includedCodeOneMatch == null)
                        continue;

                    int countryId = includedCodeOneMatch.CodeGroup.CountryId;

                    if(!importedCountriesByCountryId.TryGetValue(countryId, out importedCountry))
                    {
                        importedCountry = new ImportedCountry();
                        importedCountry.ImportedCodes = new List<ImportedCode>();
                        importedCountry.ImportedRates = new List<ImportedRate>();
                    }

                    IEnumerable<ImportedCode> includedCodes = zone.ImportedCodes.Where(x => !x.IsExcluded);
                    foreach (ImportedCode code in includedCodes)
                        importedCountry.ImportedCodes.Add(code);

                    IEnumerable<ImportedRate> includedRates = zone.ImportedRates.Where(x => !x.IsExcluded);
                    foreach (ImportedRate rate in includedRates)
                        importedCountry.ImportedRates.Add(rate);

                    importedCountriesByCountryId.Add(countryId, importedCountry);
                }
            }

            this.ImportedCountries.Set(context, importedCountriesByCountryId.Values);
        }
    }
}

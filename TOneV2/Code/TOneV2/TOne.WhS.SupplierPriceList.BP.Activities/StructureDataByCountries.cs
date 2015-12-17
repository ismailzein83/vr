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
            IEnumerable<ImportedCountry> importedCountriesList = this.ImportedCountries.Get(context);

            Dictionary<int, ImportedCountry> importedCountriesByCountryId = new Dictionary<int, ImportedCountry>();
            ImportedCountry importedCountry;

            CodeGroupManager codeGroupManager = new CodeGroupManager();

            foreach (ImportedZone zone in importedZonesList)
            {
                if(!zone.IsExecluded)
                {
                    //This should be validated when data is imported, that all codes has code groups and are linked to countries
                    int countryId = codeGroupManager.GetCodeGroup(zone.ImportedCodes.First().CodeGroupId.Value).CountryId;
                    if(!importedCountriesByCountryId.TryGetValue(countryId, out importedCountry))
                    {
                        importedCountry = new ImportedCountry();
                        importedCountry.ImportedCodes = new List<ImportedCode>();
                        importedCountry.ImportedRates = new List<ImportedRate>();
                    }

                    importedCountry.ImportedCodes.AddRange(zone.ImportedCodes);
                    importedCountry.ImportedRates.AddRange(zone.ImportedRates);

                    importedCountriesByCountryId.Add(countryId, importedCountry);
                }
            }

            this.ImportedCountries.Set(context, importedCountriesByCountryId.Values);
        }
    }
}

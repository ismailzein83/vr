using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Common.Business;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Entities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Business;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class StructureDataByCountries : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ImportedZone>> ImportedZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SupplierZone>> ExistingZoneEntities { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ImportedCountry>> ImportedCountries { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ImportedZone> importedZonesList = this.ImportedZones.Get(context);
            IEnumerable<SupplierZone> existingZoneList = this.ExistingZoneEntities.Get(context);
            PriceListCodeManager priceListCodeManager = new PriceListCodeManager();

            Dictionary<int, ImportedCountry> importedCountriesByCountryId = new Dictionary<int, ImportedCountry>();
            ImportedCountry importedCountry;

            foreach (ImportedZone zone in importedZonesList)
            {
                if (!zone.ImportedCodes.Any()) continue;

                int countryId = priceListCodeManager.GetCountryId(zone.ImportedCodes);

                if (!importedCountriesByCountryId.TryGetValue(countryId, out importedCountry))
                {
                    importedCountry = new ImportedCountry
                    {
                        CountryId = countryId,
                        ImportedZones = new List<ImportedZone>(),
                        ImportedCodes = new List<ImportedCode>(),
                        ImportedRates = new List<ImportedRate>()
                    };
                    importedCountriesByCountryId.Add(countryId, importedCountry);
                }

                foreach (ImportedCode code in zone.ImportedCodes)
                    importedCountry.ImportedCodes.Add(code);

                importedCountry.ImportedRates.Add(zone.ImportedNormalRate);

                importedCountry.ImportedZones.Add(zone);

            }

            CountryManager manager = new CountryManager();
            IEnumerable<Country> countries = manager.GetAllCountries();

            foreach (Country country in countries)
            {
                if (!importedCountriesByCountryId.TryGetValue(country.CountryId, out importedCountry) && existingZoneList.Any(x => x.CountryId == country.CountryId))
                {
                    importedCountry = new ImportedCountry();
                    importedCountry.CountryId = country.CountryId;
                    importedCountry.ImportedZones = new List<ImportedZone>();
                    importedCountry.ImportedCodes = new List<ImportedCode>();
                    importedCountry.ImportedRates = new List<ImportedRate>();
                    importedCountriesByCountryId.Add(country.CountryId, importedCountry);
                }
            }

            this.ImportedCountries.Set(context, importedCountriesByCountryId.Values);
        }
    }
}

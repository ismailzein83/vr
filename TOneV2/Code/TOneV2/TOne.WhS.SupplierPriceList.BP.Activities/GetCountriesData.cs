using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public sealed class GetCountriesData : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ImportedCountry>> ImportedCountries { get; set; }

        [RequiredArgument]
        public OutArgument<Dictionary<int, ProcessedCountryDataInfo>> CountryDataByCountryId { get; set; }
       

        protected override void Execute(CodeActivityContext context)
        {
            Dictionary<int, ProcessedCountryDataInfo> countryDataByCountryId = new Dictionary<int, ProcessedCountryDataInfo>();
            IEnumerable<ImportedCountry> importedCountries = this.ImportedCountries.Get(context);
            foreach (var country in importedCountries)
            {

                var importedCountry = countryDataByCountryId.GetRecord(country.CountryId);
                if (importedCountry == null )
                {
                        var countryData = new ProcessedCountryDataInfo
                        {
                            CountryId = country.CountryId,
                            ImportedCodes = country.ImportedCodes,
                            ImportedZones = country.ImportedZones,
                            ImportedRates = country.ImportedRates,
                            NotImportedCodes = new List<NotImportedCode>(),
                            NotImportedZones = new List<NotImportedZone>(),
                        };
                        countryDataByCountryId.Add(country.CountryId, countryData);
                }
                
            }
            this.CountryDataByCountryId.Set(context, countryDataByCountryId);
        }
    }
}

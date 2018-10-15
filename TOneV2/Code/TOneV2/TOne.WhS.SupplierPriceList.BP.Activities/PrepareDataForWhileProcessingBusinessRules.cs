using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
   public class PrepareDataForWhileProcessingBusinessRules : CodeActivity
    {
       [RequiredArgument]
       public InArgument<Dictionary<int, ProcessedCountryDataInfo>> CountryDataByCountryId { get; set; }
       [RequiredArgument]
       public OutArgument<List<CountryNotImportedCodes>> CountriesAllNotImportedCodes { get; set; }
       protected override void Execute(CodeActivityContext context)
       {
          Dictionary<int, ProcessedCountryDataInfo> countryDataByCountryId = this.CountryDataByCountryId.Get(context);
           List<CountryNotImportedCodes> countriesNotImportedCodes = new List<CountryNotImportedCodes>();
           foreach(var countryData in countryDataByCountryId.Values)
           {
               if(countryData.NotImportedCodes!= null && countryData.NotImportedCodes.Count() > 0)
               {
                   var countryNotImportedCode = new CountryNotImportedCodes()
                   {
                       CountryId = countryData.CountryId,
                       NotImportedCodes = countryData.NotImportedCodes
                   };
                   countriesNotImportedCodes.Add(countryNotImportedCode);
               }
           }

           this.CountriesAllNotImportedCodes.Set(context, countriesNotImportedCodes);
       }
    }
      
}

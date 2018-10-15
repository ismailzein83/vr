using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Entities
{
   public class AllCountriesNotImportedCodes : IRuleTarget
    {
       public List<CountryNotImportedCodes> CountriesNotImportedCodes { get; set; }
        public object Key
        {
            get { return default(object); }
        }
        public string TargetType
        {
            get { return "CountriesNotImportedCodes"; }
        }
    }
}

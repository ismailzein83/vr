using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;

namespace TOne.WhS.CodePreparation.Entities.processing
{
   public interface IProcessCountriesContext
    {
       IEnumerable<ZoneToProcess> ZonesToProcess { get; set; }
       int CountryId { get; set; }
       int SellingNumberPlanId { get; set; }
       IEnumerable<ChangedCustomerCountry> ChangedCustomerCountries { get; set; }
    }
}

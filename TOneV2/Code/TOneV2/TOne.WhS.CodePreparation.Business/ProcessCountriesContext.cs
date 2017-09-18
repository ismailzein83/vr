using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.processing;
using TOne.WhS.CodePreparation.Entities.Processing;

namespace TOne.WhS.CodePreparation.Business
{
    public class ProcessCountriesContext : IProcessCountriesContext
    {
        public IEnumerable<ZoneToProcess> ZonesToProcess { get; set; }
        public int CountryId { get; set; }
        public int SellingNumberPlanId { get; set; }
        public IEnumerable<ChangedCustomerCountry> ChangedCustomerCountries { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class DraftChangedCountries
    {
        public IEnumerable<DraftChangedCountry> Countries { get; set; }
        public DateTime EED { get; set; }
    }

    public class DraftChangedCountry
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
    }
}

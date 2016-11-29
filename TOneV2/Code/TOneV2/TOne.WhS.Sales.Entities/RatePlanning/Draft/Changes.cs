using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class Changes
    {
        public int? CurrencyId { get; set; }

        public DefaultChanges DefaultChanges { get; set; }

        public List<ZoneChanges> ZoneChanges { get; set; }

		public CountryChanges CountryChanges { get; set; }
    }
}

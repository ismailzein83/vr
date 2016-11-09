using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePLZoneChange
    {
        public int CountryId { get; set; }

        public string ZoneName { get; set; }

        public bool HasCodeChange { get; set; }

        public List<int> CustomersHavingRateChange { get; set; }
    }
}

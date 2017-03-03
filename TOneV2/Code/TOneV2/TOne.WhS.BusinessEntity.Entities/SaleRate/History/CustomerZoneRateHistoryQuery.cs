using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CustomerZoneRateHistoryQuery
    {
        public int CustomerId { get; set; }

        public string ZoneName { get; set; }

        public int CountryId { get; set; }
    }
}

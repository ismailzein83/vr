using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CustomerZones
    {
        public int CustomerZonesId { get; set; }

        public int CustomerId { get; set; }

        public List<CustomerCountry> Countries { get; set; }

        public DateTime StartEffectiveTime { get; set; }
    }

    public class CustomerCountry
    {
        public int CountryId { get; set; }

        public DateTime StartEffectiveTime { get; set; }

        public DateTime? EndEffectiveTime { get; set; }
    }

    public class CustomerCountry2 : Vanrise.Entities.IDateEffectiveSettings
    {
        public int CustomerCountryId { get; set; }

        public int CustomerId { get; set; }

        public int CountryId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}

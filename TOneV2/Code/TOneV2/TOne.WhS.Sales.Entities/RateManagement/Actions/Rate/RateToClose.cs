using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class RateToClose
    {
        public long ZoneId { get; set; }

        public string ZoneName { get; set; }

        public int? RateTypeId { get; set; }

        public decimal Rate { get; set; }

        public DateTime CloseEffectiveDate { get; set; }

        List<ExistingRate> _changedExistingRates = new List<ExistingRate>();

        public List<ExistingRate> ChangedExistingRates
        {
            get
            {
                return _changedExistingRates;
            }
        }
    }
}

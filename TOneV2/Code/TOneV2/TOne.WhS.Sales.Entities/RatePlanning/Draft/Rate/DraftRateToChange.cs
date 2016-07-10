using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Entities
{
    //TODO: remove IDateEffectiveSettings in case it is not needed anymore
    public class DraftRateToChange : Vanrise.Entities.IDateEffectiveSettings
    {
        //TODO: to remove later if there is no need to, the replacement is zone name
        public long ZoneId { get; set; }

        public string ZoneName { get; set; }

        public Decimal NormalRate { get; set; }

        public Dictionary<int, Decimal> OtherRates { get; set; }

        public int? CurrencyId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        //TODO: to be removed after converting the logic to a process
        List<ExistingRate> _changedExistingRates = new List<ExistingRate>();
        public List<ExistingRate> ChangedExistingRates
        {
            get { return _changedExistingRates; }
        }
    }
}

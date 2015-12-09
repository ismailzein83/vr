using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities.RateManagement;

namespace TOne.WhS.Sales.Entities
{
    public class NewRate : Vanrise.Entities.IDateEffectiveSettings
    {
        public long ZoneId { get; set; }

        public Decimal NormalRate { get; set; }

        public Dictionary<int, Decimal> OtherRates { get; set; }

        public int? CurrencyId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public RateChangeType ChangeType { get; set; }

        List<ExistingRate> _changedExistingRates = new List<ExistingRate>();
        public List<ExistingRate> ChangedExistingRates
        {
            get { return _changedExistingRates; }
        }
    }

    public class RateChange
    {
        public long RateId { get; set; }

        public DateTime? EED { get; set; }
    }
}

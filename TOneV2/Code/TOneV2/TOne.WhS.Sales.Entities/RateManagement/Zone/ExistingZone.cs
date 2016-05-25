using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class ExistingZone : IZone
    {
        public BusinessEntity.Entities.SaleZone ZoneEntity { get; set; }

        public long ZoneId
        {
            get { return ZoneEntity.SaleZoneId; }
        }

        public string Name
        {
            get { return ZoneEntity.Name; }
        }

        public int CountryId
        {
            get { return ZoneEntity.CountryId; }
        }

        public DateTime BED
        {
            get { return ZoneEntity.BED; }
        }

        public DateTime? EED
        {
            get { return ZoneEntity.EED; }
        }

        List<NewRate> _newRates = new List<NewRate>();

        public List<NewRate> NewRates
        {
            get {
                return _newRates;
            }
        }

        List<ExistingRate> _existingRates = new List<ExistingRate>();
        public List<ExistingRate> ExistingRates
        {
            get
            {
                return _existingRates;
            }
        }
    }
}

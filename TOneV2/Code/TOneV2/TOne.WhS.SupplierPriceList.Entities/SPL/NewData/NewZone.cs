using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class NewZone : IZone
    {
        public long ZoneId { get; set; }
        public bool IsExcluded { get; set; }
        public int CountryId { get; set; }

        public string Name { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        List<NewCode> _newCodes = new List<NewCode>();
        public List<NewCode> NewCodes
        {
            get
            {
                return _newCodes;
            }
        }

        List<NewRate> _newRates = new List<NewRate>();
        public List<NewRate> NewRates
        {
            get
            {
                return _newRates;
            }
        }

        List<NewZoneService> _newZoneServices = new List<NewZoneService>();
        public List<NewZoneService> NewZoneServices
        {
            get
            {
                return _newZoneServices;
            }
        }
    }

    public class NewZonesByName : Dictionary<string, List<NewZone>>
    {

    }
}

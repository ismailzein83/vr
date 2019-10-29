using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public enum TechnicalAddressNumberType { NearbyNumber = 1, FDBNumber = 2, DPNumber = 3 }
    public class GetTechnicalAddressOutput
    {
        public long AreaId { get; set; }

        public long SiteId { get; set; }

        public int RegionId { get; set; }

        public int CityId { get; set; }

        public int TownId { get; set; }

        public long StreetId { get; set; }

        public string BuildingDetails { get; set; }

        public List<GetTechnicalAddressOutputTechnologyItem> TechnologyItems { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public class Node
    {
        public long NodeId { get; set; }
        public int ModelId { get; set; }
        public string Number { get; set; }
        public Guid StatusId { get; set; }
        public long SiteId { get; set; }
        public Guid NodeTypeId { get; set; }
        public long AreaId { get; set; }
        public string Notes { get; set; }
        public long StreetId { get; set; }
        public string Building { get; set; }
        public int? BuildingSizeId { get; set; }
        public string BlockNumber { get; set; }
        public int RegionId { get; set; }
        public int CityId { get; set; }
        public int TownId { get; set; }
    }
}

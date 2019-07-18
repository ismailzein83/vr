using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class AddressData
    {
        public Guid CityId { get; set; }
        public Guid ProvinceId { get; set; }
        public Guid AreaId { get; set; }
        public Guid TownId { get; set; }
    }
}

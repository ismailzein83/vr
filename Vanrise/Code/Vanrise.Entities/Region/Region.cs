using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class Region
    {
        public int RegionId { get; set; }

        public string Name { get; set; }

        public int CountryId { get; set; }

        public RegionSettings Settings { get; set; }
    }

    public class RegionSettings
    {

    }
}

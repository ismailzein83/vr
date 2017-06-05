﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class City
    {
        public int CityId { get; set; }

        public string Name { get; set; }

        public int CountryId { get; set; }
        public string SourceId { get; set; }
        public CitySettings Settings { get; set; }
    }

    public class CitySettings
    {
        public int? RegionId { get; set; }
    }
}

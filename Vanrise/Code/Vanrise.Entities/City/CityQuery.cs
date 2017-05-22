﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class CityQuery
    {
        public string Name { get; set; }

        public List<int> CountryIds { get; set; }

        public List<int> RegionIds { get; set; }
    }
}

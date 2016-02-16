﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class CustomerZones
    {
        public int CustomerZonesId { get; set; }

        public int CustomerId { get; set; }

        public List<CustomerCountry> Countries { get; set; }

        public DateTime StartEffectiveTime { get; set; }
    }

    public class CustomerCountry
    {
        public int CountryId { get; set; }
    }
}

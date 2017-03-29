﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePricelistRateChange
    {
        public string ZoneName { get; set; }
        public int CountryId { get; set; }
        public Decimal Rate { get; set; }
        public Decimal? RecentRate { get; set; }
        public RateChangeType ChangeType { get; set; }
        public int PricelistId { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }
}

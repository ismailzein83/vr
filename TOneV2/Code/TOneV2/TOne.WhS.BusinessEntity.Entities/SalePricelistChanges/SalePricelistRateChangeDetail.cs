﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities.SalePricelistChanges
{
    public class SalePricelistRateChangeDetail
    {
        public string ZoneName { get; set; }
        public decimal Rate { get; set; }
        public decimal? RecentRate { get; set; }
        public int? RateTypeId { get; set; }
        public IEnumerable<int> ServicesId { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
        public RateChangeType ChangeType { get; set; }
        public string CurrencySymbol { get; set; }
        public long ZoneId { get; set; }
        public string RateTypeName { get; set; }
    }
}

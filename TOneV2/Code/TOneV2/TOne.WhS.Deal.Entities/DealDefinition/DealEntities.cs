﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public struct DealZoneGroup
    {
        public int DealId { get; set; }
        public int ZoneGroupNb { get; set; }
    }

    public struct DealZoneGroupTierRate
    {
        public int DealId { get; set; }
        public int ZoneGroupNb { get; set; }
        public int TierNb { get; set; }
        public int RateTierNb { get; set; }
    }

    public struct AccountZoneGroup
    {
        public int AccountId { get; set; }
        public long ZoneId { get; set; }
    }

    public class DealZoneGroupTier
    {
        public int TierNumber { get; set; }
        public decimal? Volume { get; set; }
        public decimal Rate { get; set; }
        public Dictionary<long, decimal> ExceptionRates { get; set; }
    }

    public class DealZoneGroupData
    {
        public int DealID { get; set; }

        public int ZoneGroupNb { get; set; }

        public bool IsSale { get; set; }

        public decimal? TotalZoneGroupDurationInSeconds { get; set; }
    }
} 
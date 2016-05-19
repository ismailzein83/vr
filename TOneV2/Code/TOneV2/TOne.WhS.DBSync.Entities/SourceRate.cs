﻿using System;
using Vanrise.Entities.EntitySynchronization;

namespace TOne.WhS.DBSync.Entities
{
    public class SourceRate : ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }

        public int? PriceListId { get; set; }
        public int? ZoneId { get; set; }
        public decimal? Rate { get; set; }
        public decimal? OffPeakRate { get; set; }
        public decimal? WeekendRate { get; set; }
        public int? Change { get; set; }
        public int? ServicesFlag { get; set; }
        public DateTime? BeginEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public string Notes { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public enum ZoneChangeType
    {
        [Description("Not Changed")]
        NotChanged = 0,

        [Description("New")]
        New = 1,

        [Description("Deleted")]
        Deleted = 2,

        [Description("Re-Opened")]
        ReOpened = 3,

        [Description("Renamed")]
        Renamed = 4
    };


    public class ZoneRatePreview
    {
        public int CountryId { get; set; }
        public string ZoneName { get; set; }
        public string RecentZoneName { get; set; }
        public ZoneChangeType ChangeTypeZone { get; set; }
        public DateTime ZoneBED { get; set; }
        public DateTime? ZoneEED { get; set; }
        public decimal? CurrentRate { get; set; }
        public DateTime? CurrentRateBED { get; set; }
        public DateTime? CurrentRateEED { get; set; }
        public decimal? ImportedRate { get; set; }
        public DateTime? ImportedRateBED { get; set; }
        public RateChangeType ChangeTypeRate { get; set; }

    }


    public class ZoneRatePreviewDetail
    {
        public string ZoneName { get; set; }
        public string RecentZoneName { get; set; }
        public ZoneChangeType ChangeTypeZone { get; set; }
        public DateTime ZoneBED { get; set; }
        public DateTime? ZoneEED { get; set; }
        public decimal? CurrentRate { get; set; }
        public DateTime? CurrentRateBED { get; set; }
        public DateTime? CurrentRateEED { get; set; }
        public decimal? ImportedRate { get; set; }
        public DateTime? ImportedRateBED { get; set; }
        public RateChangeType ChangeTypeRate { get; set; }

        public int NewCodes { get; set; }

        public int DeletedCodes { get; set; }

        public int CodesMovedTo { get; set; }

        public int CodesMovedFrom { get; set; }


    }
  
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
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
        public decimal? SystemRate { get; set; }
        public DateTime? SystemRateBED { get; set; }
        public DateTime? SystemRateEED { get; set; }
        public decimal? ImportedRate { get; set; }
        public DateTime? ImportedRateBED { get; set; }
        public RateChangeType ChangeTypeRate { get; set; }
        public List<int> SystemServiceIds { get; set; }
        public DateTime? SystemServicesBED { get; set; }
        public DateTime? SystemServicesEED { get; set; }
        public List<int> ImportedServiceIds { get; set; }
        public DateTime? ImportedServicesBED { get; set; }
        public ZoneServiceChangeType ZoneServicesChangeType { get; set; }

    }


    public class    ZoneRatePreviewDetail
    {
        public string ZoneName { get; set; }
        public string RecentZoneName { get; set; }
        public ZoneChangeType ChangeTypeZone { get; set; }
        public DateTime ZoneBED { get; set; }
        public DateTime? ZoneEED { get; set; }
        public decimal? SystemRate { get; set; }
        public DateTime? SystemRateBED { get; set; }
        public DateTime? SystemRateEED { get; set; }
        public decimal? ImportedRate { get; set; }
        public DateTime? ImportedRateBED { get; set; }
        public RateChangeType ChangeTypeRate { get; set; }
        public List<int> SystemServiceIds { get; set; }
        public DateTime? SystemServicesBED { get; set; }
        public DateTime? SystemServicesEED { get; set; }
        public List<int> ImportedServiceIds { get; set; }
        public DateTime? ImportedServicesBED { get; set; }
        public ZoneServiceChangeType ZoneServicesChangeType { get; set; }

        public int NewCodes { get; set; }

        public int DeletedCodes { get; set; }

        public int CodesMovedTo { get; set; }

        public int CodesMovedFrom { get; set; }


    }
  
}

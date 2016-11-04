﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum ZoneServiceChangeType
    {

        [Description("Not Changed")]
        NotChanged = 0,

        [Description("New")]
        New = 1,

        [Description("Deleted")]
        Deleted = 2,
    }

    public enum SupplierEntityServiceSource : byte { Supplier, SupplierZone }

    public class SupplierEntityService
    {
        public List<ZoneService> Services { get; set; }

        public SupplierEntityServiceSource Source { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }


    public class SupplierEntityServiceDetail
    {
        public SupplierEntityService Entity { get; set; }
        public String ZoneName { get; set; }
        public List<int> Services { get; set; }
    }


    public class SupplierZoneService : IBusinessEntityInfo
    {
        public long SupplierZoneServiceId { get; set; }

        public long ZoneId { get; set; }

        public int PriceListId { get; set; }

        public List<ZoneService> ReceivedServices { get; set; }

        public List<ZoneService> EffectiveServices { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public string SourceId { get; set; }
    }

    public class SupplierDefaultService : IBusinessEntityInfo
    {
        public long SupplierZoneServiceId { get; set; }

        //TODO: to be removed and Supplier Price List Id to add
        public int PriceListId { get; set; }

        public int? SupplierId { get; set; }

        public List<ZoneService> ReceivedServices { get; set; }

        public List<ZoneService> EffectiveServices { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public string SourceId { get; set; }
    }
}

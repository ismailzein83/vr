using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TOne.WhS.BusinessEntity.Entities
{

    public class SupplierZoneServiceDetail
    {
        public SupplierZoneService Entity { get; set; }
        public string SupplierZoneName { get; set; }
        public List<int> Services { get; set; }
    }
}

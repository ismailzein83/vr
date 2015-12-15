using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Entities;
namespace Tests.Sample
{
    class ExistingCodes
    {
        public DateTime BED { get; set; }
        public ChangedCode ChangedCode { get; set; }
        public SupplierCode CodeEntity { get; set; }
        public DateTime? EED { get; set; }
        public ExistingZone ParentZone { get; set; }
    }
}

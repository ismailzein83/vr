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
    public class OtherRatePreview
    {
        public string ZoneName { get; set; }
        public decimal? SystemRate { get; set; }
        public DateTime? SystemRateBED { get; set; }
        public DateTime? SystemRateEED { get; set; }
        public decimal? ImportedRate { get; set; }
        public DateTime? ImportedRateBED { get; set; }
        public int RateTypeId { get; set; }
        public RateChangeType ChangeTypeRate { get; set; }
        public bool IsExcluded { get; set; }

    }


    public class OtherRatePreviewDetail
    {
        public OtherRatePreview Entity { get; set; }

        public string RateTypeDescription { get; set; }
    }
  
}

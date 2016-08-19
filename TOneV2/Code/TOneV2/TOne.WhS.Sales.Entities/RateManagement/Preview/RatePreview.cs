using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class RatePreview
    {
        public string ZoneName { get; set; }

        public int? RateTypeId { get; set; }

        public decimal? CurrentRate { get; set; }

        public bool? IsCurrentRateInherited { get; set; }

        public decimal? NewRate { get; set; }

        public RateChangeType ChangeType { get; set; }

        public DateTime? EffectiveOn { get; set; }

        public DateTime? EffectiveUntil { get; set; }
    }

    public class RatePreviewDetail
    {
        public RatePreview Entity { get; set; }

        public string RateTypeName { get; set; }

        public string ChangeTypeDescription { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
   public class OtherRatesPreview
    {
        public string ZoneName { get; set; }

        public int? RateTypeId { get; set; }

        public decimal? CurrentRate { get; set; }

        public decimal? NewRate { get; set; }

        public RateChangeType ChangeType { get; set; }

        public DateTime? BED { get; set; }

        public DateTime? EED { get; set; }

        public int CurrencyId { get; set; }
    }
   public class OtherRatesPreviewDetail
   {
       public RatePreview Entity { get; set; }

       public string RateTypeName { get; set; }

       public string ChangeTypeDescription { get; set; }

       public string CurrencySymbol { get; set; }
   }
}

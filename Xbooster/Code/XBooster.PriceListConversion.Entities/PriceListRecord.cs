using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBooster.PriceListConversion.Entities
{
    public class PriceListRecord
    {
        public string Zone { get; set; }
        public List<PriceListCode> Codes { get; set; }
        public decimal Rate { get; set; }
        public DateTime? RateEffectiveDate { get; set; }
    }
    public class PriceListCode
    {
        public string Code { get; set; }
        public DateTime? CodeEffectiveDate { get; set; }
    }
}

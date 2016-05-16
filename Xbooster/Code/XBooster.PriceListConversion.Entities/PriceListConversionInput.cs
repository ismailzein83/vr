using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBooster.PriceListConversion.Entities
{
    public class PriceListConversionInput
    {
        public int InputFileId { get; set; }
        public InputPriceListSettings InputPriceListSettings { get; set; }
        public int OutputPriceListTemplateId { get; set; }
    }
}

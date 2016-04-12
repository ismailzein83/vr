using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelConversion.Entities
{
    public class PriceListConversion
    {
        public int InputFileId { get; set; }
        public InputPriceListSettings InputPriceListSettings { get; set; }
        public OutputPriceListConfiguration OutputPriceListConfiguration { get; set; }
    }
}

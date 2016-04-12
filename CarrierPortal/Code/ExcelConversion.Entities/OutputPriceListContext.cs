using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelConversion.Entities
{
    public class OutputPriceListContext : IOutputPriceListContext
    {
        public List<PriceListRecord> Records{ get; set;}
    }
}

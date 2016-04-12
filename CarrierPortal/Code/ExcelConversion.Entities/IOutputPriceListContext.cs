using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelConversion.Entities
{
    public interface IOutputPriceListContext
    {
        List<PriceListRecord> Records { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XBooster.PriceListConversion.Entities;

namespace XBooster.PriceListConversion.Business
{
    public class OutputPriceListExecutionContext : IOutputPriceListExecutionContext
    {
        public List<PriceListRecord> Records { get; set; }
    }
}

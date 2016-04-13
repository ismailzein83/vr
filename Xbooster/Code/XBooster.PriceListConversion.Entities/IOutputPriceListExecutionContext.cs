using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBooster.PriceListConversion.Entities
{
    public interface IOutputPriceListExecutionContext
    {
        List<PriceListRecord> Records { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelConversion.Entities
{
    public abstract class OutputPriceListConfiguration
    {
        public int ConfigId { get; set; }

        public abstract byte[] Execute(IOutputPriceListContext context);
    }
}

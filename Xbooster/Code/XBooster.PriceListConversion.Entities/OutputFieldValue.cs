using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBooster.PriceListConversion.Entities
{
    public abstract class OutputFieldValue
    {
        public int ConfigId { get; set; }
        public abstract void Execute(IFieldValueExecutionContext context);
    }
}

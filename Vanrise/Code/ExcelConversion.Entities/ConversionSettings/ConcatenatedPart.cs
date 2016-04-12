using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelConversion.Entities
{
    public abstract class ConcatenatedPart
    {
        public int ConfigId { get; set; }

        public abstract string GetPartText(IGetConcatenatedPartTextContext context);
    }
}

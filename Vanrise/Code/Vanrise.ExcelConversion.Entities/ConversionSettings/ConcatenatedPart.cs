using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.ExcelConversion.Entities
{
    public abstract class ConcatenatedPart
    {
        public virtual Guid ConfigId { get; set; }

        public abstract string GetPartText(IGetConcatenatedPartTextContext context);
    }
}

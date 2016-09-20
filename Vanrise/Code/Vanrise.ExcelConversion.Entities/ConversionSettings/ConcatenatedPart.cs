using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.ExcelConversion.Entities
{
    public abstract class ConcatenatedPart
    {
        public abstract Guid ConfigId { get; }

        public abstract string GetPartText(IGetConcatenatedPartTextContext context);
    }
}

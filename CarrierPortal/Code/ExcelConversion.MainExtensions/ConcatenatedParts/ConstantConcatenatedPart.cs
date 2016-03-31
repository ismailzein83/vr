using ExcelConversion.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelConversion.MainExtensions.ConcatenatedParts
{
    public class ConstantConcatenatedPart : ConcatenatedPart
    {
        public string Constant { get; set; }

        public override string GetPartText(IGetConcatenatedPartTextContext context)
        {
            return this.Constant;
        }
    }
}

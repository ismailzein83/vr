using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.ExcelConversion.Entities;

namespace Vanrise.ExcelConversion.MainExtensions.ConcatenatedParts
{
    public class ConstantConcatenatedPart : ConcatenatedPart
    {
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("096b0021-49a1-44f6-b601-406372dc5038"); } }
        public string Constant { get; set; }

        public override string GetPartText(IGetConcatenatedPartTextContext context)
        {
            return this.Constant;
        }
    }
}

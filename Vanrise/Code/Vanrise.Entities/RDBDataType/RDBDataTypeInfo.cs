using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class RDBDataTypeInfo
    {
        public RDBDataType Value { get; set; }
        public string Description { get; set; }
        public bool RequireSize { get; set; }
        public bool RequirePrecision { get; set; }
        public bool HasSizeRequired { get; set; }
        public bool HasPrecisionRequired { get; set; }
    }
}

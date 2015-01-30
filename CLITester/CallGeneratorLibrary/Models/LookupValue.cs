using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CallGeneratorLibrary
{
    public partial class LookupValue : ICloneable
    {
        public object Clone()
        {
            return (LookupValue)this.MemberwiseClone();
        }
    }
}

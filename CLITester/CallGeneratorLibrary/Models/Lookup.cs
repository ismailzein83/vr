using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CallGeneratorLibrary
{
    public partial class Lookup : ICloneable
    {
        public object Clone()
        {
            return (Lookup)this.MemberwiseClone();
        }
    }
}

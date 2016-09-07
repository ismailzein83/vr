using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class VRConcatenatedPartSettings<T> where T : class
    {
        public int ConfigId { get; set; }
        public abstract string GetPartText(T context);
    }
}

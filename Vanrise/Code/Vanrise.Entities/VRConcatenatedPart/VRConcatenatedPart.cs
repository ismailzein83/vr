using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRConcatenatedPart<T> where T : class
    {
        public string PartTitle { get; set; }
        public VRConcatenatedPartSettings<T> Settings { get; set; }
    }
}

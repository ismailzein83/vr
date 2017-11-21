using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRLocalizationTextResourceQuery
    {
        public string ResourceKey { get; set; }
        public List<Guid> ModuleIds { get; set; }

    }
}

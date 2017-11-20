using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRLocalizationTextResource
    {
        public Guid VRLocalizationTextResourceId { get; set; }

        public string ResourceKey { get; set; }

        public Guid ModuleId { get; set; }

        public VRLocalizationTextResourceSettings Settings { get; set; }
    }

    public class VRLocalizationTextResourceSettings
    {
        public string DefaultValue { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    #region public
    public class VRLocalizationTextResourceDetail
    {
        public Guid VRLocalizationTextResourceId { get; set; }
        public string ResourceKey { get; set; }
        public Guid ModuleId { get; set; }
        public string ModuleName { get; set; }

    }
    #endregion
}

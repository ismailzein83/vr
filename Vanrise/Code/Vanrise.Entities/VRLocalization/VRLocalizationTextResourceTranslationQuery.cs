using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities.VRLocalization
{
   public class VRLocalizationTextResourceTranslationQuery
    {
        public List<Guid> ResourceIds { get; set; }

        public List<Guid> LanguageIds { get; set; }
    }
}

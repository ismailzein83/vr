using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities.VRLocalization
{
   public class VRLocalizationTextResourceTranslationDetail
    {
        public Guid VRLocalizationTextResourceTranslationId { get; set; }

        public Guid ResourceId { get; set; }

        public Guid LanguageId { get; set; }

        public VRLocalizationTextResourceTranslationSettings Settings { get; set; }
        public string ResourceKey { get; set; }
        public string languageName { get; set; }
    }
}

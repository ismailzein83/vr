using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRLocalizationTextResourceTranslation
    {
        public Guid VRLocalizationTextResourceTranslationId { get; set; }

        public Guid ResourceId { get; set; }

        public Guid LanguageId { get; set; }

        public VRLocalizationTextResourceTranslationSettings Settings { get; set; }
    }

    public class VRLocalizationTextResourceTranslationSettings
    {
        public string Value { get; set; }
    }

    public class VRLocalizationTextResourceTranslationsById : Dictionary<Guid, VRLocalizationTextResourceTranslation>
    {

    }
}

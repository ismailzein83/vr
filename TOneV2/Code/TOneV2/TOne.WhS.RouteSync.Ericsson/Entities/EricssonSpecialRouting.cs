using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Ericsson.Entities
{
    public abstract class EricssonSpecialRoutingSetting
    {
        public abstract Guid ConfigId { get; }
    }

    public class EricssonSpecialRoutingServiceLanguage : EricssonSpecialRoutingSetting
    {
        public override Guid ConfigId { get { return new Guid("A9497EF9-2074-4A1E-A999-08253B68F448"); } }

        List<CodeGroupSuffix> CodeGroupSuffixes { get; set; }
    }

    public class CodeGroupSuffix
    {
        public string Suffix { get; set; }
    }

    public class EricssonSpecialRoutingSettingConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_RouteSync_EricssonSpecialRoutingServiceLanguageConfig";

        public string Editor { get; set; }
    }
}

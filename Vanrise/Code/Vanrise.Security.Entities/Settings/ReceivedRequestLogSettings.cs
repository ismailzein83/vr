using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class ReceivedRequestLogSettings
    {
        public bool EnableLogging { get; set; }
        public bool EnableParametersLogging { get; set; }
        public bool EnableRequestHeaderLogging { get; set; }
        public bool EnableRequestBodyLogging { get; set; }
        public bool EnableResponseHeaderLogging { get; set; }
        public bool EnableResponseBodyLogging { get; set; }
        public ModuleFilterSettings Filter { get; set; }
    }

    public abstract class ModuleFilterSettings
    {
        public abstract Guid ConfigId { get; }
    }

    public class AllModuleExcept : ModuleFilterSettings
    {
        public override Guid ConfigId { get { return new Guid("18E1ADF1-7B1E-4A4C-8C56-E7EFC1306FB1"); } }

        public List<string> ExcludedModules { get; set; }
    }

    public class SpecificModule : ModuleFilterSettings
    {
        public override Guid ConfigId { get { return new Guid("ED560AFD-9292-4D8E-8F95-29AAEFDA013C"); } }
        public List<string> IncludedModules { get; set; }
    }
}

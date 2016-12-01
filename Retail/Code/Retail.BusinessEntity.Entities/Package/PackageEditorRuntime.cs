using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class PackageEditorRuntime
    {
        public Package Entity { get; set; }

        public PackageExtendedSettingsEditorRuntime ExtendedSettingsEditorRuntime { get; set; }
    }
}

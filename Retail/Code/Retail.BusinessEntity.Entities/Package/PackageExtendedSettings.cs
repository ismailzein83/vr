using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class PackageExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public virtual PackageExtendedSettingsEditorRuntime GetEditorRuntime()
        {
            return null;
        }
    }

    public abstract class PackageExtendedSettingsEditorRuntime
    {

    }
}

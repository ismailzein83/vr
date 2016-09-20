using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class VolumeDefinitionSettings
    {
        public abstract Guid ConfigId { get; }

        public virtual string VolumeSettingsEditor { get; set; }
    }
}

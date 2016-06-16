using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class VolumeDefinitionSettings
    {
        public int ConfigId { get; set; }

        public virtual string VolumeSettingsEditor { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class DurationVolumeDefinitionSettings : VolumeDefinitionSettings
    {
        public override string VolumeSettingsEditor
        {
            get
            {
                return "retail-be-durationvolumesettings";
            }
            set
            {

            }
        }

        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }
    }
}

using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.SMS.Entities
{
    public class TextMessageVolumeDefinitionSettings: VolumeDefinitionSettings
    {
        public override string VolumeSettingsEditor
        {
            get
            {
                return "retail-sms-textmessagevolume-settings";
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class SMSMessageType : Vanrise.Entities.VRComponentType<SMSMessageTypeSettings>
    {

    }

    public class SMSMessageTypeSettings: VRComponentTypeSettings
    {
        public VRObjectVariableCollection Objects { get; set; }

        public override Guid VRComponentTypeConfigId {
            get
            {
                return new Guid("80791C8A-5F81-4D2E-B3D7-4240CF967FA0");
            }
        }
    }
   
   
}

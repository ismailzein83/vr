using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class SMSMessageType
    {
        public Guid SMSMessageTypeId { get; set; }

        public string Name { get; set; }

        public SMSMessageTypeSettings Settings { get; set; }
    }

    public class SMSMessageTypeSettings
    {
        public VRObjectVariableCollection Objects { get; set; }
    }
}

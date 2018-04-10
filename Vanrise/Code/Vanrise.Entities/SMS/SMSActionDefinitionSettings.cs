using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class SMSActionDefinitionSettings
    {
        public SMSSendHandler SendHandler { get; set; }

        public Guid SMSMessageTypeId { get; set; }
    }
}

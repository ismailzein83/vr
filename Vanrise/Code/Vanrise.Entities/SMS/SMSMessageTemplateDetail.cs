using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities.SMS
{
    public class SMSMessageTemplateDetail
    {
        public SMSMessageTemplate Entity { get; set; }

        public string CreatorName { get; set; }

        public string LastModifierName { get; set; }
    }
}

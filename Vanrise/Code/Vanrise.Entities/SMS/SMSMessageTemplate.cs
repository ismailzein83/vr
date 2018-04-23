using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class SMSMessageTemplate
    {
        public Guid SMSMessageTemplateId { get; set; }

        public string Name { get; set; }

        public Guid SMSMessageTypeId { get; set; }

        public SMSMessageTemplateSettings Settings { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedTime { get; set; }

        public int LastModifiedBy { get; set; }

        public DateTime LastModifiedTime { get; set; }
    }

    public class SMSMessageTemplateSettings
    {
        public VRExpression MobileNumber { get; set; }

        public VRExpression Message { get; set; }
    }
}

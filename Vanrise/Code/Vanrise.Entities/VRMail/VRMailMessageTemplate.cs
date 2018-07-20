using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRMailMessageTemplate
    {
        public Guid VRMailMessageTemplateId { get; set; }

        public string Name { get; set; }

        public Guid VRMailMessageTypeId { get; set; }

        public VRMailMessageTemplateSettings Settings { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    public class VRMailMessageTemplateSettings
    {
        public VRExpression From { get; set; }
        public VRExpression To { get; set; }

        public VRExpression CC { get; set; }

        public VRExpression BCC { get; set; }

        public VRExpression Subject { get; set; }

        public VRExpression Body { get; set; }
    }
}

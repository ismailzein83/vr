using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRMailMessageTemplateQuery
    {
        public string Name { get; set; }
        public List<Guid> MailMessageType { get; set; }
    }
}



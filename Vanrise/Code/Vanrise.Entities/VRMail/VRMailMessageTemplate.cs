using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRMailMessageTemplate
    {
        public Guid VRMailMessageTypeId { get; set; }

        public List<VRObjectPropertyVariable> Variables { get; set; }

        public VRExpression To { get; set; }

        public VRExpression CC { get; set; }

        public VRExpression Subject { get; set; }

        public VRExpression Body { get; set; }
    }
}

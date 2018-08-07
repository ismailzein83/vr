using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class VRWorkflowClass
    {
        public Guid VRWorkflowClassId { get; set; }

        public string Namespace { get; set; }

        public string Code { get; set; } 
    }

    public class VRWorkflowClassCollection : List<VRWorkflowClass>
    {

    }
}
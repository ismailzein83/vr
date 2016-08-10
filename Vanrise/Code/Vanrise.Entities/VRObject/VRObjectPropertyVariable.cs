using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRObjectPropertyVariable
    {
        public string VariableName { get; set; }

        public string Description { get; set; }

        public string ObjectName { get; set; }

        public VRObjectPropertyEvaluator PropertyEvaluator { get; set; }
    }
}

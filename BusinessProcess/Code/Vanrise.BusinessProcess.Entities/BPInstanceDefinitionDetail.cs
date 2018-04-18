using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPInstanceDefinitionDetail
    {
        public BPDefinition Entity { get; set; }

        public bool AllowCancel { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Security.Entities;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPDefinitionArgumentState
    {
        public Guid BPDefinitionID { get; set; }

        public BaseProcessInputArgument InputArgument { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPVisualItemDefinition
    {
        public BPVisualItemDefinitionSettings Settings { get; set; }
    }

    public abstract class BPVisualItemDefinitionSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract string Editor { get; }
    }
}

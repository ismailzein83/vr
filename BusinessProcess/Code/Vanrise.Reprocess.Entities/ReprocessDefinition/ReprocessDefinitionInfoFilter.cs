using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Reprocess.Entities
{
    public class ReprocessDefinitionInfoFilter
    {
        public List<Guid> ExcludedReprocessDefinitionIds { get; set; }
    }
}

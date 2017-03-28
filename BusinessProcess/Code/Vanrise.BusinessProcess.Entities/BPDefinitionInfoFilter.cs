using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPDefinitionInfoFilter
    {
        public List<IBPDefinitionFilter> Filters { get; set; }
    }

    public interface IBPDefinitionFilter
    {
        bool IsMatched(IBPDefinitionFilterContext context);
    }

    public interface IBPDefinitionFilterContext
    {
        Guid BPDefinitionId { get; }
    }

    public class BPDefinitionFilterContext : IBPDefinitionFilterContext
    {
        public Guid BPDefinitionId { get; set; }
    }
}

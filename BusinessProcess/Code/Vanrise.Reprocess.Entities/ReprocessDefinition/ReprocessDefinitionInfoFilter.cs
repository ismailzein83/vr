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

        public List<IReprocessDefinitionFilter> Filters { get; set; }
    }

    public interface IReprocessDefinitionFilter
    {
        bool IsMatched(IReprocessDefinitionFilterContext context);
    }

    public interface IReprocessDefinitionFilterContext
    {
        ReprocessDefinition ReprocessDefinition { get; }
    }

    public class ReprocessDefinitionFilterContext : IReprocessDefinitionFilterContext
    {
        public ReprocessDefinition ReprocessDefinition { get; set; }
    }
}
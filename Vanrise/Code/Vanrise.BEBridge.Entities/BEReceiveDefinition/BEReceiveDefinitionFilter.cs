using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BEBridge.Entities
{
    public class BEReceiveDefinitionFilter
    {

        public List<IBEReceiveDefinitionFilter> Filters { get; set; }
    }

    public interface IBEReceiveDefinitionFilter
    {
        bool IsMatched(IBEReceiveDefinitionFilterContext context);
    }

    public interface IBEReceiveDefinitionFilterContext
    {
        Guid BEReceiveDefinitionId { get; }
    }

    public class BEReceiveDefinitionFilterContext : IBEReceiveDefinitionFilterContext
    {
        public Guid BEReceiveDefinitionId { get; set; }
    }
}

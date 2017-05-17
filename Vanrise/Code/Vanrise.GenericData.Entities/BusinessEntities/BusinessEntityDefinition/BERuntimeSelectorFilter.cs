using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public abstract class BERuntimeSelectorFilter
    {
        public abstract bool IsMatched(IBERuntimeSelectorFilterSelectorFilterContext context);
    }

    public interface IBERuntimeSelectorFilterSelectorFilterContext
    {
        Guid BusinessEntityDefinitionId { get; }

        object BusinessEntityId { get; }
    }

    public class BERuntimeSelectorFilterSelectorFilterContext : IBERuntimeSelectorFilterSelectorFilterContext
    {
        public Guid BusinessEntityDefinitionId { get; set; }

        public object BusinessEntityId { get; set; }
    }
}

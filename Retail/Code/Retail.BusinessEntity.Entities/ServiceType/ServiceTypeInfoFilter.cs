using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ServiceTypeInfoFilter
    {
        public Guid? AccountBEDefinitionId { get; set; }

        public IEnumerable<Guid> ExcludedServiceTypeIds { get; set; }

        public bool IncludeHiddenServiceTypes { get; set; }

        public List<IServiceTypeFilter> Filters { get; set; }
    }
}

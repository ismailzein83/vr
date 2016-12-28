using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public interface IServiceTypeFilter
    {
        bool IsMatched(IPackageDefinitionServiceTypeFilterContext context);
    }
    public interface IPackageDefinitionServiceTypeFilterContext
    {
        Entities.ServiceType entityDefinition { get;}
    }
}

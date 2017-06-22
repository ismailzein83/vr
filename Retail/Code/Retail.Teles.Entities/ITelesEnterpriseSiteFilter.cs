using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Entities
{
    public interface ITelesEnterpriseSiteFilter
    {
        bool IsExcluded(ITelesEnterpriseSiteFilterContext context);
    }
    public interface ITelesEnterpriseSiteFilterContext
    {
        dynamic EnterpriseSiteId { get; }
        Guid AccountBEDefinitionId { get; set; }
    }
}

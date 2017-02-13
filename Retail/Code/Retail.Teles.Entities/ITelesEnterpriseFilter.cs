using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Entities
{
    public interface ITelesEnterpriseFilter
    {
        bool IsExcluded(ITelesEnterpriseFilterContext context);
    }
    public interface ITelesEnterpriseFilterContext
    {
        dynamic EnterpriseId { get; }
        Guid AccountBEDefinitionId { get; set; }
    }
}

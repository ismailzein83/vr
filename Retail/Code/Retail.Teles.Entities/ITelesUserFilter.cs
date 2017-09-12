using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Entities
{
    public interface ITelesUserFilter
    {
        bool IsExcluded(ITelesUserFilterContext context);
    }
    public interface ITelesUserFilterContext
    {
        string UserId { get; }
        Guid AccountBEDefinitionId { get; set; }
    }
}

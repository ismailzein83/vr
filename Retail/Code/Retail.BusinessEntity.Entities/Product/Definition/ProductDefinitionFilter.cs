using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ProductDefinitionFilter
    {
        public Guid? AccountBEDefinitionId { get; set; }
        public bool IncludeHiddenProductDefinitions { get; set; }

        public List<IProductDefinitionFilter> Filters { get; set; }
    }

    public interface IProductDefinitionFilter
    {
        bool IsMatched(IProductDefinitionFilterContext context);
    }

    public interface IProductDefinitionFilterContext
    {
        Guid ProductDefinitionId { get; }
    }

    public class ProductDefinitionFilterContext : IProductDefinitionFilterContext
    {
        public Guid ProductDefinitionId { get; set; }
    }
}
